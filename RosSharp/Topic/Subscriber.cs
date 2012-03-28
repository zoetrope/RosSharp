using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    public sealed class Subscriber<TDataType> : ISubscriber, IObservable<TDataType> 
        where TDataType : IMessage, new ()
    {
        public Subscriber(string name, string nodeId)
        {
            Name = name;
            var dummy = new TDataType();
            Type = dummy.MessageType;

            NodeId = nodeId;
        }

        void ISubscriber.UpdatePublishers(List<Uri> publishers)
        {
            //TODO: 同じPublisherに対する処理
            var slaves = publishers.Select(x => new SlaveClient(x));

            //TODO: 並列に動かすべき？
            slaves.ToList()
                .ForEach(slave =>
                         slave.RequestTopicAsync(NodeId, Name, new object[1] {new string[1] {"TCPROS"}})
                             .ContinueWith(task => Connect(task.Result)));


        }

        private void Connect(TopicParam param)
        {
            //TODO: serverを複数持てるようにする
            var server = new RosTopicServer<TDataType>(Name,NodeId);
            server.Start(param, _subject);//TODO: こいつは非同期に。

            var handler = OnConnected;
            if (handler != null)
            {
                handler();
            }
        }
        public event Action OnConnected;

        public string NodeId { get; private set; }
        public string Name { get; private set; }

        public string Type { get; private set; }

        private Subject<TDataType> _subject = new Subject<TDataType>();

        public IDisposable Subscribe(IObserver<TDataType> observer)
        {
            return _subject.Subscribe(observer);
        }

    }

}
