using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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
            //TODO: SendAsyncは別スレッドで動かすほうが良い
            _rosTopics.ForEach(x => x.SendAsync(value).First());
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
