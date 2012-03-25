using RosSharp.Message;

namespace RosSharp.Service
{
    public interface IService<TRequest, TResponse> where TRequest : IMessage where TResponse : IMessage
    {
        string ServiceType { get; }
        string Md5Sum { get; }
        string ServiceDefinition { get; }

        //TResponse CreateResponse();
        //TRequest CreateRequest();
    }
}
