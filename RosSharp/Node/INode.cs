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

        TService CreateProxy<TService>(string serviceName)
            where TService : IService, new();

        void RemoveServiceProxy(string serviceName);

        IDisposable RegisterService<TService>(string serviceName, TService service)
            where TService : IService, new();

        void RemoveService(string serviceName);


        Parameter<T> GetParameter<T>(string paramName);
    }
}
