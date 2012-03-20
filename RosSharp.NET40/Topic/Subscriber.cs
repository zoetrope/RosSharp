using System;
using System.IO;
using System.Reactive.Linq;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    public sealed class Subscriber<TDataType> : ISubscriber, IObservable<TDataType> 
        where TDataType : IMessage, new ()
    {
        private RosTcpClient _tcpClient;
        public Subscriber(string name, string nodeId)
        {
            Name = name;
            var dummy = new TDataType();
            Type = dummy.MessageType;

            NodeId = nodeId;
        }

        internal void Connect(TopicParam param) //TODO:非同期待ちにすべき
        {
            _tcpClient = new RosTcpClient();
            var ret = _tcpClient.ConnectAsync(param.HostName, param.PortNumber).First();

            //TODO: RosTopicに委譲

            var last = _tcpClient.ReceiveAsync()
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
            var data = stream.ToArray();

            _tcpClient.SendAsync(data).First();

            var test = last.First();
        }

        public string NodeId { get; private set; }
        public string Name { get; private set; }

        public string Type { get; private set; }
        
        public void UpdatePublishers()
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<TDataType> observer)
        {
            return _tcpClient.ReceiveAsync()
                .Select(x =>
                        {
                            var data = new TDataType();
                            var br = new BinaryReader(new MemoryStream(x));
                            br.ReadInt32();
                            data.Deserialize(br);
                            return data;
                        })
                .Subscribe(observer);
        }
    }
}
