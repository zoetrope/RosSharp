#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    public sealed class Parameter<T> : IObservable<T>, IParameter
    {
        private readonly IParameterCoverter<T> _converter;
        private readonly ParameterServerClient _parameterServerClient;
        private readonly Uri _slaveUri;
        private Subject<T> _parameterSubject;

        internal Parameter(string nodeId, string paramName, Uri slaveUri, ParameterServerClient client)
        {
            NodeId = nodeId;
            Name = paramName;
            _slaveUri = slaveUri;

            _parameterServerClient = client;


            if (typeof (T).IsPrimitive || typeof (T) == typeof (string))
            {
                _converter = new PrimitiveParameterConverter<T>();
            }
            else if (typeof (T) == typeof (List<>))
            {
                _converter = new ListParameterConverter<T>();
            }
            else if (typeof (T) == typeof (DictionaryParameter))
            {
                _converter = new DictionaryParameterConverter<T>();
            }
            else
            {
                throw new ArgumentException("invalid Type Argument");
            }
        }

        public string NodeId { get; private set; }
        public string Name { get; private set; }

        public T Value
        {
            get
            {
                var result = _parameterServerClient.GetParamAsync(NodeId, Name).Result;
                return _converter.ConvertTo(result);
            }
            set { _parameterServerClient.SetParamAsync(NodeId, Name, _converter.ConvertFrom(value)).Wait(); }
        }

        #region IObservable<T> Members

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_parameterSubject == null)
            {
                _parameterSubject = new Subject<T>();
                var disposable = _parameterSubject.Subscribe(observer);

                var subsTask = _parameterServerClient.SubscribeParamAsync(NodeId, _slaveUri, Name);

                subsTask.ContinueWith(
                    t =>
                    {
                        try
                        {
                            if (t.Result is XmlRpcStruct && ((XmlRpcStruct) t.Result).Keys.Count == 0)
                            {
                                // if subscribe to parameter that are not set, receive an empty XmlRpcStruct.
                                return;
                            }

                            var val = _converter.ConvertTo(t.Result);
                            _parameterSubject.OnNext(val);
                        }
                        catch (Exception ex)
                        {
                            _parameterSubject.OnError(ex);
                            _parameterSubject.OnCompleted();
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                subsTask.ContinueWith(
                    t =>
                    {
                        _parameterSubject.OnError(t.Exception.InnerException);
                        _parameterSubject.OnCompleted();
                    }, TaskContinuationOptions.OnlyOnFaulted);

                return disposable;
            }
            else
            {
                return _parameterSubject.Subscribe(observer);
            }
        }

        #endregion

        #region IParameter Members

        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        void IParameter.Update(object value)
        {
            try
            {
                var data = _converter.ConvertTo(value);
                _parameterSubject.OnNext(data);
            }
            catch (Exception ex)
            {
                _parameterSubject.OnError(ex);
                _parameterSubject.OnCompleted();
            }
        }

        public Task DisposeAsync()
        {
            if (_parameterSubject != null)
            {
                _parameterSubject.Dispose();
                _parameterSubject = null;
            }

            return _parameterServerClient.UnsubscribeParamAsync(NodeId, _slaveUri, Name);
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        #endregion

        internal Task InitializeAsync()
        {
            return _parameterServerClient.HasParamAsync(NodeId, Name)
                .ContinueWith(task =>
                {
                    if (task.Result)
                    {
                        return _parameterServerClient.GetParamAsync(NodeId, Name);
                    }
                    else
                    {
                        return _parameterServerClient.SetParamAsync(NodeId, Name, new XmlRpcStruct());
                    }
                })
                .Unwrap();
        }
    }
}