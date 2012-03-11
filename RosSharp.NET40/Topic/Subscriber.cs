using System;
using System.IO;
using System.Reactive.Linq;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    public class Subscriber<TDataType> : ISubscriber, IObservable<TDataType> 
        where TDataType : IMessage, new ()
    {
        

        private RosTcpClient _tcpClient;
        public Subscriber(string name, TopicParam param)
        {
            Name = name;
            var dummy = new TDataType();
            Type = dummy.MessageType;

            _tcpClient = new RosTcpClient();
            var ret = _tcpClient.ConnectAsync(param.HostName, param.PortNumber).First();

            var headerSerializer = new TcpRosHeaderSerializer<SubscriberResponseHeader>();

            //TODO: RosTopicに委譲

            _tcpClient.ReceiveAsync()
                .Take(1)
                .Select(x => headerSerializer.Deserialize(new MemoryStream(x)))
                .Subscribe(x => Console.WriteLine(x.topic + "/" + x.type));

            

            var header = new SubscriberHeader()
            {
                callerid = "test",
                topic = "/chatter",
                md5sum = dummy.Md5Sum,
                type = dummy.MessageType
            };

            var serializer = new TcpRosHeaderSerializer<SubscriberHeader>();

            var stream = new MemoryStream();

            serializer.Serialize(stream, header);
            var data = stream.ToArray();

            _tcpClient.SendAsync(data).First();
        }

        public string Name { get; private set; }

        public string Type { get; private set; }
        public void UpdatePublishers()
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<TDataType> observer)
        {
            return _tcpClient.ReceiveAsync()
                .Select(x => {
                    var data = new TDataType();
                    data.Deserialize(new MemoryStream(x));
                    return data;
                })
                .Subscribe(observer);
        }
    }
}
