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

namespace RosSharp.Parameter
{
    public class Parameter<T> : IObservable<T>, IParameter
    {
        private IParameterCoverter<T> _converter;
        private ParameterServerClient _parameterServerClient;
        private Subject<T> _parameterSubject;
        private Uri _slaveUri;

        public Parameter(string nodeId, string paramName, Uri slaveUri, ParameterServerClient client)
        {
            NodeId = nodeId;
            Name = paramName;
            _slaveUri = slaveUri;

            _parameterServerClient = client;
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
                _parameterServerClient.SubscribeParamAsync(NodeId, _slaveUri, Name)
                    .ContinueWith(
                        task =>
                        {
                            var val = _converter.ConvertTo(task.Result);
                            _parameterSubject.OnNext(val);
                        });
                //TODO: SetParamしてないのにSubscribeするとおかしなデータが来る
            }

            return _parameterSubject.Subscribe(observer);
        }

        #endregion

        #region IParameter Members

        void IParameter.Update(object value)
        {
            var data = _converter.ConvertTo(value);
            _parameterSubject.OnNext(data);
        }

        public void Dispose()
        {
            if (_parameterSubject != null)
            {
                _parameterServerClient.UnsubscribeParamAsync(NodeId, _slaveUri, Name).Wait();
                _parameterSubject = null;
            }
        }

        #endregion
    }

    internal interface IParameterCoverter<T>
    {
        T ConvertTo(object value);
        object ConvertFrom(T value);
    }

    internal class PrimitiveParameterConverter<T> : IParameterCoverter<T>
    {
        #region IParameterCoverter<T> Members

        public T ConvertTo(object value)
        {
            return (T) value;
        }

        public object ConvertFrom(T value)
        {
            return value;
        }

        #endregion
    }

    internal class ListParameterConverter<T> : IParameterCoverter<T>
    {
        #region IParameterCoverter<T> Members

        public T ConvertTo(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertFrom(T value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class DictionaryParameterConverter<T> : IParameterCoverter<T>
    {
        #region IParameterCoverter<T> Members

        public T ConvertTo(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertFrom(T value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}