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
        private Subject<T> _parameterSubject;
        private readonly Uri _slaveUri;

        internal Parameter(string nodeId, string paramName, Uri slaveUri, ParameterServerClient client)
        {
            NodeId = nodeId;
            Name = paramName;
            _slaveUri = slaveUri;

            _parameterServerClient = client;


            if(typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                _converter = new PrimitiveParameterConverter<T>();
            }
            else if(typeof(T) == typeof(List<>))
            {
                _converter = new ListParameterConverter<T>();
            }
            else if (typeof(T) == typeof(DictionaryParameter))
            {
                _converter = new DictionaryParameterConverter<T>();
            }
            else
            {
                throw new ArgumentException("invalid Type Argument");
            }
        }

        internal Task Initialize()
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
                .Unwrap()
                .ContinueWith(task =>
                {

                });

        }

        public string NodeId { get; private set; }
        public string Name { get; private set; }

        public T Value
        {
            get
            {
                //TODO: エラー処理は？
                var result = _parameterServerClient.GetParamAsync(NodeId, Name).Result;
                return _converter.ConvertTo(result);
            }
            set
            {
                //TODO: エラー処理は？
                _parameterServerClient.SetParamAsync(NodeId, Name, _converter.ConvertFrom(value)).Wait();
            }
        }

        #region IObservable<T> Members

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_parameterSubject == null)
            {
                //TODO: ここで初期化してよい？Subscribeが呼ばれるするまでSubscribeParamを実行せずにすむので効率的？
                _parameterSubject = new Subject<T>();
                var disposable = _parameterSubject.Subscribe(observer);
                _parameterServerClient.SubscribeParamAsync(NodeId, _slaveUri, Name)
                    .ContinueWith(
                        task =>
                        {
                            //TODO: SetParamしてないのにSubscribeするとおかしなデータが来る
                            //TODO: Convertが失敗したときの処理
                            var val = _converter.ConvertTo(task.Result);
                            _parameterSubject.OnNext(val);
                        }); //TODO: SubscribeParamAsyncがエラーの時は？
                
                return disposable;
            }
            else
            {
                return _parameterSubject.Subscribe(observer);
            }
            
        }

        #endregion

        #region IParameter Members

        void IParameter.Update(object value)
        {
            //TODO: Convertが失敗したときの処理
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
}
