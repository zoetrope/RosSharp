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
                return _parameterServerClient.GetParamAsync(NodeId, Name).Select(_converter.ConvertTo).First();
            }
            set
            {
                _parameterServerClient.SetParamAsync(NodeId, Name, _converter.ConvertFrom(value)).First();
            }
        }

        private Subject<T> _parameterSubject;
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_parameterSubject == null)
            {
                _parameterSubject = new Subject<T>();
                var value = _parameterServerClient.SubscribeParamAsync(NodeId, _slaveUri, Name).First();
                var data = _converter.ConvertTo(value); //TODO: SetParamしてないのにSubscribeするとおかしなデータが来る
                _parameterSubject.OnNext(data);
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
                _parameterServerClient.UnsubscribeParamAsync(NodeId, _slaveUri, Name).First();
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
