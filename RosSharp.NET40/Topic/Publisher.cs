using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using RosSharp.Message;

namespace RosSharp.Topic
{
    public class Publisher<TDataType> : IPublisher, IObserver<TDataType> where TDataType : IMessage, new()
    {
        private List<RosTopic<TDataType>> _rosTopics = new List<RosTopic<TDataType>>();

        public Publisher(string name)
        {
            var dummy = new TDataType();

            Name = name;
            Type = dummy.MessageType;
        }

        public string Name { get; private set; }

        public string Type { get; private set; }

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
