using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    internal sealed class RosTopicClient<TDataType> : IDisposable
        where TDataType : IMessage, new()
    {
        private RosTcpClient _tcpClient;

        public string NodeId { get; private set; }
        public string TopicName { get; private set; }

        public bool IsConnected { get;private set; }

        public RosTopicClient(string nodeId, string topicName)
        {
            IsConnected = false;

            NodeId = nodeId;
            TopicName = topicName;

        }

        public void Dispose()
        {
            _tcpClient.Dispose();
            IsConnected = false;
        }

        public Task<int> SendTaskAsync(TDataType data)
        {
            if(!IsConnected)
            {
                throw new InvalidOperationException("Is not Connected.");
            }
            
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(data.SerializeLength);
            data.Serialize(bw);
            return _tcpClient.SendTaskAsync(ms.ToArray());
        }


        public void Start(Socket socket)
        {
            _tcpClient = new RosTcpClient(socket);

            _tcpClient.ReceiveAsync()
                .Take(1)
                .Subscribe(OnReceiveHeader);
        }

        private void OnReceiveHeader(byte[] data)
        {
            var reqHeader = TcpRosHeaderSerializer.Deserialize(new MemoryStream(data));
            //TODO: 受信したデータのチェック

            var temp = new TDataType();

            var resHeader = new
            {
                callerid = NodeId,
                latching = "0",
                md5sum = temp.Md5Sum,
                message_definition = temp.MessageDefinition,
                topic = TopicName,
                type = temp.MessageType
            };

            var ms = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(ms, resHeader);

            _tcpClient.SendTaskAsync(ms.ToArray()).Wait(); //TODO: Waitでよい？

            IsConnected = true;
        }

    }
}
