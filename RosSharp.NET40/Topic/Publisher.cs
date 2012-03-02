using System;
using System.Collections.Generic;
using RosSharp.Message;

namespace RosSharp.Topic
{
    public class Publisher<TDataType> : ITopic, IObserver<TDataType> where TDataType : IMessage, new()
    {
        private List<RosTopic<TDataType>> _rosTopics = new List<RosTopic<TDataType>>();

        public Publisher()
        {
        }

        public GraphName TopicName
        {
            get { throw new NotImplementedException(); }
        }

        public string TopicMessageType
        {
            get { throw new NotImplementedException(); }
        }

        public void OnNext(TDataType value)
        {
            _rosTopics.ForEach(x => x.Send(value));
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        internal void AddTopic(RosTopic<TDataType> rosTopic)
        {
            _rosTopics.Add(rosTopic);
        }
    }
}
