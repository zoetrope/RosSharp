using System;
using System.Threading.Tasks;
using RosSharp.Message;

namespace RosSharp.Service
{
    public interface IService
    {
        string ServiceType { get; }
        string Md5Sum { get; }
        string ServiceDefinition { get; }

        IMessage CreateRequest();
        IMessage CreateResponse();
        IMessage Invoke(IMessage req);
        void SetAction(Func<IMessage, IMessage> action);
    }

    public interface ITypedService<in TRequest, TResponse> : IService
        where TRequest : IMessage, new()
        where TResponse : IMessage, new()
    {
        TResponse Invoke(TRequest req);
        Task<TResponse> InvokeTaskAsync(TRequest req);
    }

}
