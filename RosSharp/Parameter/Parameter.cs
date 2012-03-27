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
        private ParameterServerClient _parameterServerClient;
        public string NodeId { get; private set; }
        public string Name { get; private set; }
        private Uri _slaveUri;

        private IParameterCoverter<T> _converter;

        public Parameter(string nodeId, string paramName, Uri slaveUri, ParameterServerClient client)
        {
            NodeId = nodeId;
            Name = paramName;
            _slaveUri = slaveUri;

            _parameterServerClient = client;
        }

        public T Value
        {
            get
            {
                var result = _parameterServerClient.GetParamAsync(NodeId, Name).Result;
                return _converter.ConvertTo(result);
            }
            set
            {
                _parameterServerClient.SetParamAsync(NodeId, Name,_converter.ConvertFrom(value)).Wait();
            }
        }

        private Subject<T> _parameterSubject;
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

    }

    internal interface IParameterCoverter<T>
    {
        T ConvertTo(object value);
        object ConvertFrom(T value);
    }

    internal class PrimitiveParameterConverter<T> : IParameterCoverter<T>
    {
        public T ConvertTo(object value)
        {
            return (T) value;
        }
        public object ConvertFrom(T value)
        {
            return value;
        }
    }

    internal class ListParameterConverter<T> : IParameterCoverter<T>
    {
        public T ConvertTo(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertFrom(T value)
        {
            throw new NotImplementedException();
        }
    }

    internal class DictionaryParameterConverter<T> : IParameterCoverter<T>
    {
        public T ConvertTo(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertFrom(T value)
        {
            throw new NotImplementedException();
        }
    }
}
