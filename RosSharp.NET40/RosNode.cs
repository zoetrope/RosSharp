using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class RosNode : INode
    {
        public Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName)
        {
            throw new NotImplementedException();
        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(GraphName topicName)
        {
            throw new NotImplementedException();
        }

        public Publisher<TDataType> CreatePublisher<TDataType>(string topicName)
        {
            throw new NotImplementedException();
        }

        public Publisher<TDataType> CreatePublisher<TDataType>(GraphName topicName)
        {
            throw new NotImplementedException();
        }
    }
}
