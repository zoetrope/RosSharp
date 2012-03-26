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

            slaves.ToList()
                .ForEach(slave =>
                         slave.RequestTopicAsync(NodeId, Name, new object[1] {new string[1] {"TCPROS"}})
                             .Subscribe(topicParam => Connect(topicParam)));


        }

        private void Connect(TopicParam param)
        {
            var tcpClient = new RosTcpClient();
            tcpClient.ConnectTaskAsync(param.HostName, param.PortNumber).Wait();

            //TODO: RosTopicに委譲

            var last = tcpClient.ReceiveAsync()
                .Take(1)
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .PublishLast();
                //.Subscribe(x => Console.WriteLine(x.topic + "/" + x.type));

            last.Connect();

            var dummy = new TDataType();
            var header = new 
            {
                callerid = NodeId,
                topic = Name,
                md5sum = dummy.Md5Sum,
                type = dummy.MessageType
            };

            var stream = new MemoryStream();

            TcpRosHeaderSerializer.Serialize(stream, header);
            var sendData = stream.ToArray();

            tcpClient.SendTaskAsync(sendData).Wait();

            var test = last.First();

            tcpClient.ReceiveAsync()
                .Select(x =>
                        {
                            var data = new TDataType();
                            var br = new BinaryReader(new MemoryStream(x));
                            br.ReadInt32();
                            data.Deserialize(br);
                            return data;
                        })
                .Subscribe(_subject);


            var handler = Connected;
            if (handler != null)
            {
                handler();
            }
        }
        public event Action Connected;

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
