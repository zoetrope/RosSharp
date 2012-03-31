using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    internal sealed class RosTopicClient<TDataType> : IDisposable
        where TDataType : IMessage, new()
    {
        private TcpRosClient _client;
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public string NodeId { get; private set; }
        public string TopicName { get; private set; }

        public bool Connected { get;private set; }

        public RosTopicClient(string nodeId, string topicName)
        {
            Connected = false;

            NodeId = nodeId;
            TopicName = topicName;
        }

        public void Dispose()
        {
            _client.Dispose();
            Connected = false;
        }

        public Task<int> SendTaskAsync(TDataType data)
        {
            if(!Connected)
            {
                _logger.Error(m => m("Not Connected"));
                throw new InvalidOperationException("Not Connected");
            }
            
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(data.SerializeLength);
            data.Serialize(bw);
            return _client.SendTaskAsync(ms.ToArray());
        }


        public Task StartAsync(Socket socket)
        {
            _client = new TcpRosClient(socket);

            return Task.Factory.StartNew(
                () =>
                _client.ReceiveAsync()
                    .Take(1)
                    .Timeout(TimeSpan.FromMilliseconds(ROS.TopicTimeout))
                    .Select(OnReceivedHeader).First());
        }

        private Unit OnReceivedHeader(byte[] data)
        {
            var dummy = new TDataType();
            dynamic reqHeader = TcpRosHeaderSerializer.Deserialize(new MemoryStream(data));

            if(reqHeader.topic != TopicName)
            {
                _logger.Error(m => m("TopicName mismatch error, expected={0} actual={1}", TopicName, reqHeader.topic));
                throw new RosTopicException("TopicName mismatch error");
            }
            if (reqHeader.type != dummy.MessageType)
            {
                _logger.Error(m => m("TopicType mismatch error, expected={0} actual={1}", dummy.MessageType, reqHeader.type));
                throw new RosTopicException("TopicType mismatch error");
            }
            if (reqHeader.md5sum != dummy.Md5Sum)
            {
                _logger.Error(m => m("MD5Sum mismatch error, expected={0} actual={1}", dummy.Md5Sum, reqHeader.md5sum));
                throw new RosTopicException("MD5Sum mismatch error");
            }

            var resHeader = new
            {
                callerid = NodeId,
                latching = "0",
                md5sum = dummy.Md5Sum,
                message_definition = dummy.MessageDefinition,
                topic = TopicName,
                type = dummy.MessageType
            };

            var ms = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(ms, resHeader);

            _client.SendTaskAsync(ms.ToArray()).Wait();

            Connected = true;

            return Unit.Default;
        }

    }
}
