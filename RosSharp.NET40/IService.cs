using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public interface IService<TRequest, TResponse> where TRequest : IMessage where TResponse : IMessage
    {
        string ServiceType { get; }
        string Md5Sum { get; }
        string ServiceDefinition { get; }

        TResponse CreateResponse();
        TRequest CreateRequest();
    }
}
