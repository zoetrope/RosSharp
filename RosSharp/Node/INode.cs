using System;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.Topic;
using RosSharp.Parameter;

namespace RosSharp.Node
{
    public interface INode : IDisposable
    {
        Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) 
            where TDataType : IMessage, new();

        void RemoveSubscriber(string topicName);

        Publisher<TDataType> CreatePublisher<TDataType>(string topicName) 
            where TDataType : IMessage, new();

        void RemovePublisher(string topicName);

        Func<TRequest, IObservable<TResponse>> CreateProxy<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new();

        void RemoveServiceProxy(string serviceName);

        IDisposable RegisterService<TService, TRequest, TResponse>(string serviceName, Func<TRequest, TResponse> service)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new();

        void RemoveService(string serviceName);


        Parameter<T> GetParameter<T>(string paramName);
    }
}
