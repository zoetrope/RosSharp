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
        private RosTcpClient _tcpClient;

        public RosTopic(Socket socket)
        {
            _tcpClient = new RosTcpClient(socket);

            _tcpClient.ReceiveAsync()
                .Take(1)
                .Subscribe(OnReceiveHeader);
        }

        protected void OnReceiveHeader(byte[] data)
        {
            Console.WriteLine(data.Length);

            var s = new TcpRosHeaderSerializer<SubscriberHeader>();
            var h = s.Deserialize(new MemoryStream(data));

            var temp = new TDataType();

            var header = new SubscriberResponseHeader()
            {
                callerid = "/test",
                latching = "0",
                md5sum = temp.Md5Sum,
                message_definition = temp.MessageDefinition,
                topic = "/chatter",
                type = temp.MessageType
            };

            var serializer = new TcpRosHeaderSerializer<SubscriberResponseHeader>();

            var ms = new MemoryStream();
            serializer.Serialize(ms, header);

            _tcpClient.SendAsync(ms.ToArray()).First();

        }

        public void Send(TDataType data)
        {
            var ms = new MemoryStream();
            data.Serialize(ms);
            var array = ms.ToArray();
            _tcpClient.SendAsync(ms.ToArray()).First();
        }
    }
}
