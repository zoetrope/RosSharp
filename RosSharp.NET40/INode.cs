using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public interface INode
    {
        Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) where TDataType : IMessage, new();

        Publisher<TDataType> CreatePublisher<TDataType>(string topicName) where TDataType : IMessage, new();

        Func<TRequest, TResponse> CreateProxy<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new();

    }
}
