using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public interface INode
    {
        Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName);
        Subscriber<TDataType> CreateSubscriber<TDataType>(GraphName topicName);

        Publisher<TDataType> CreatePublisher<TDataType>(string topicName);
        Publisher<TDataType> CreatePublisher<TDataType>(GraphName topicName);

    }
}
