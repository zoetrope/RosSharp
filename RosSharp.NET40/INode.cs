using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public interface INode
    {
        Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) where TDataType : IMessage, new();
        Subscriber<TDataType> CreateSubscriber<TDataType>(GraphName topicName) where TDataType : IMessage, new();

        Publisher<TDataType> CreatePublisher<TDataType>(string topicName) where TDataType : IMessage, new();
        Publisher<TDataType> CreatePublisher<TDataType>(GraphName topicName) where TDataType : IMessage, new();

    }
}
