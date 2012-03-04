using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    public class RosTopic<TDataType> where TDataType:IMessage, new()
    {
        private readonly RosTcpClient _tcpClient;

        public string NodeId { get; set; }
        public string TopicName { get; set; }

        public RosTopic(Socket socket, string nodeId, string topicName)
        {
            NodeId = nodeId;
            TopicName = topicName;

            _tcpClient = new RosTcpClient(socket);

            _tcpClient.ReceiveAsync()
                .Take(1)
                .Subscribe(OnReceiveHeader);
        }

        protected void OnReceiveHeader(byte[] data)
        {
            var s = new TcpRosHeaderSerializer<SubscriberHeader>();
            var h = s.Deserialize(new MemoryStream(data));

            var temp = new TDataType();

            var header = new SubscriberResponseHeader()
            {
                callerid = NodeId,
                latching = "0",
                md5sum = temp.Md5Sum,
                message_definition = temp.MessageDefinition,
                topic = TopicName,
                type = temp.MessageType
            };

            var serializer = new TcpRosHeaderSerializer<SubscriberResponseHeader>();

            var ms = new MemoryStream();
            serializer.Serialize(ms, header);

            _tcpClient.SendAsync(ms.ToArray()).First();

        }

        public IObservable<SocketAsyncEventArgs> SendAsync(TDataType data)
        {
            var ms = new MemoryStream();
            data.Serialize(ms);
            return _tcpClient.SendAsync(ms.ToArray());
        }
    }
}
