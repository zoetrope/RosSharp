using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace RosSharp
{
    public class Subscriber<TDataType> : ITopic, IObservable<TDataType> where TDataType : IMessage, new ()
    {
        private RosTcpClient _tcp;
        public Subscriber(TopicParam param)
        {
            _tcp = new RosTcpClient();
            var ret = _tcp.Connect(param.HostName, param.PortNumber).First();

            var headerSerializer = new TcpRosHeaderSerializer<SubscriberResponseHeader>();

            _tcp.ReceiveAsObservable()
                .Take(1)
                .Select(x => headerSerializer.Deserialize(new MemoryStream(x)))
                .Subscribe(x => Console.WriteLine(x.topic + "/" + x.type));

            var msg = new TDataType();

            var header = new SubscriberHeader()
            {
                callerid = "test",
                topic = "/chatter",
                md5sum = msg.Md5Sum,
                type = msg.DataType
            };

            var serializer = new TcpRosHeaderSerializer<SubscriberHeader>();

            var stream = new MemoryStream();

            serializer.Serialize(stream, header);
            var data = stream.ToArray();

            _tcp.Send(data).First();
        }
        
        public GraphName TopicName
        {
            get { throw new NotImplementedException(); }
        }

        public string TopicMessageType
        {
            get { throw new NotImplementedException(); }
        }

        public IDisposable Subscribe(IObserver<TDataType> observer)
        {
            var serializer = new MessageSerializer<TDataType>();

            return _tcp.ReceiveAsObservable()
                .Select(x => serializer.Deserialize(new MemoryStream(x)))
                .Subscribe(observer);
        }
    }
}
