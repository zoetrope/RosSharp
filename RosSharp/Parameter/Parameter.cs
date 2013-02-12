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
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common.Logging;
using CookComputing.XmlRpc;
using RosSharp.Utility;

namespace RosSharp.Parameter
{
    /// <summary>
    /// Base class for Ros Parameter Management
    /// </summary>
    /// <typeparam name="TParam">Parameter Type. </typeparam>
    public abstract class Parameter<TParam> : IObservable<TParam>, IParameter
    {
        internal IParameterCoverter<TParam> _converter;
        private ILog _logger = LogManager.GetCurrentClassLogger();

        protected ParameterServerClient _parameterServerClient;
        protected Subject<TParam> _parameterSubject;
        protected Uri _slaveUri;

        public string NodeId { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// Timeout Exception
        /// </summary>
        public TParam Value
        {
            get
            {
                GetAsync().Wait(TimeSpan.FromSeconds(Ros.TopicTimeout));
                return GetAsync().Result;
            }
            set { SetAsync(value).Wait(TimeSpan.FromSeconds(Ros.TopicTimeout)); }
        }

        #region IObservable<T> Members

        public IDisposable Subscribe(IObserver<TParam> observer)
        {
            if (_parameterSubject == null)
            {
                _parameterSubject = new Subject<TParam>();
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
                            _logger.Error("Convert Error", ex);
                            _parameterSubject.OnError(ex);
                            _parameterSubject.OnCompleted();
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                subsTask.ContinueWith(
                    t =>
                    {
                        _logger.Error("Failed to SubscribeParam", t.Exception.InnerException);
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

        public async Task DisposeAsync()
        {
            if (_parameterSubject != null)
            {
                _parameterSubject.Dispose();
                _parameterSubject = null;
            }

            var handler = Disposing;
            Disposing = null;

            await _parameterServerClient.UnsubscribeParamAsync(NodeId, _slaveUri, Name);
            await handler(Name);
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        void IParameter.Update(object value)
        {
            if (_parameterSubject == null)
            {
                return;
            }

            try
            {
                var data = _converter.ConvertTo(value);
                _parameterSubject.OnNext(data);
            }
            catch (Exception ex)
            {
                _logger.Error("Convert Error", ex);
                _parameterSubject.OnError(ex);
                _parameterSubject.OnCompleted();
            }
        }

        async Task IParameter.InitializeAsync(string nodeId, string paramName, Uri slaveUri, ParameterServerClient client)
        {
            NodeId = nodeId;
            _logger = RosOutLogManager.GetCurrentNodeLogger(NodeId);

            Name = paramName;
            _slaveUri = slaveUri;

            _parameterServerClient = client;

            var result = await _parameterServerClient.HasParamAsync(NodeId, Name);
            if (result)
            {
                await _parameterServerClient.GetParamAsync(NodeId, Name);
            }
            else
            {
                await _parameterServerClient.SetParamAsync(NodeId, Name, new XmlRpcStruct());
            }
        }

        #endregion

        public async Task<TParam> GetAsync()
        {
            var result = await _parameterServerClient.GetParamAsync(NodeId, Name);
            return _converter.ConvertTo(result);
        }

        public async Task SetAsync(TParam value)
        {
            await _parameterServerClient.SetParamAsync(NodeId, Name, _converter.ConvertFrom(value));
        }
    }

    public sealed class PrimitiveParameter<TParam> : Parameter<TParam>
    {
        public PrimitiveParameter()
        {
            _converter = new PrimitiveParameterConverter<TParam>();
        }
    }

    public sealed class ListParameter<TElement> : Parameter<List<TElement>>
    {
        public ListParameter()
        {
            _converter = new ListParameterConverter<TElement>();
        }
    }

    public sealed class DynamicParameter : Parameter<DynamicParameterObject>
    {
        public DynamicParameter()
        {
            _converter = new DynamicParameterConverter();
        }
    }
}