using System;
using System.Threading.Tasks;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.Topic;
using RosSharp.Parameter;

namespace RosSharp.Node
{
    public interface INode : IDisposable
    {
        Task<Subscriber<TDataType>> CreateSubscriber<TDataType>(string topicName) 
            where TDataType : IMessage, new();

        Task RemoveSubscriber(string topicName);

        Task<Publisher<TDataType>> CreatePublisher<TDataType>(string topicName) 
            where TDataType : IMessage, new();

        Task RemovePublisher(string topicName);

        Task<TService> CreateProxy<TService>(string serviceName)
            where TService : IService, new();

        Task RegisterService<TService>(string serviceName, TService service)
            where TService : IService, new();

        Task RemoveService(string serviceName);


        Parameter<T> GetParameter<T>(string paramName);
    }
}
