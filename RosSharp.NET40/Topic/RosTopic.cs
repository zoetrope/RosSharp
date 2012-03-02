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

            var s = new TcpRosHeaderSerializer<SubscriberResponseHeader>();
            var h = s.Deserialize(new MemoryStream(data));

            var header = new SubscriberResponseHeader()
            {
                callerid = "/test",
                latching = "0",
                md5sum = "992ce8a1687cec8c8bd883ec73ca41d1",
                message_definition = "string data",
                topic = "/chatter",
                type = "std_msgs/String"
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

            _tcpClient.SendAsync(ms.ToArray()).First();
        }
    }
}
