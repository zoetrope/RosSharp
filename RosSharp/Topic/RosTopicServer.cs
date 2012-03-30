using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    internal sealed class RosTopicServer<TDataType> : IDisposable
        where TDataType : IMessage, new()
    {
        private RosTcpClient _tcpClient;
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public string NodeId { get; private set; }
        public string TopicName { get; private set; }

        public RosTopicServer(string nodeId, string topicName)
        {
            NodeId = nodeId;
            TopicName = topicName;
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }

        public Task<IObservable<TDataType>> StartAsync(TopicParam param)
        {
            _tcpClient = new RosTcpClient();

            var tcs = new TaskCompletionSource<IObservable<TDataType>>();


            _tcpClient.ConnectTaskAsync(param.HostName, param.PortNumber)
                .ContinueWith(t =>
                {
                    t.Wait();
                    if (t.IsFaulted) tcs.SetException(t.Exception.InnerException);
                    else if (t.IsCanceled) tcs.SetCanceled();
                    else tcs.SetResult(OnConnected());
                });

            return tcs.Task;
        }


        private IObservable<TDataType> OnConnected()
        {
            Console.WriteLine("OnConnected");
            var last = _tcpClient.ReceiveAsync()
                .Take(1)
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .PublishLast();

            last.Connect();

            var dummy = new TDataType();
            var header = new
            {
                callerid = NodeId,
                topic = TopicName,
                md5sum = dummy.Md5Sum,
                type = dummy.MessageType
            };

            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, header);

            return _tcpClient.SendTaskAsync(stream.ToArray()) //ヘッダを送信
                .ContinueWith(task => last.SelectMany(OnReceivedHeader) //ヘッダの返信を受信
                ).Result;
        }

        private IObservable<TDataType> OnReceivedHeader(dynamic header)
        {
            var dummy = new TDataType();

            if (header.topic != TopicName)
            {
                _logger.Error(m => m("TopicName mismatch error, expected={0} but actual={1}", TopicName, header.topic));
                throw new RosTopicException("TopicName mismatch error");
            }
            if (header.type != dummy.MessageType)
            {
                _logger.Error(m => m("TopicType mismatch error, expected={0} but actual={1}", dummy.MessageType, header.type));
                throw new RosTopicException("TopicType mismatch error");
            }
            if (header.md5sum != dummy.Md5Sum)
            {
                _logger.Error(m => m("MD5Sum mismatch error, expected={0} but actual={1}", dummy.Md5Sum, header.md5sum));
                throw new RosTopicException("MD5Sum mismatch error");
            }

            return _tcpClient.ReceiveAsync().Select(Deserialize); //その後はストリーミングでデータ受信
        }

        private TDataType Deserialize(byte[] x)
        {
            var data = new TDataType();
            var br = new BinaryReader(new MemoryStream(x));
            var len = br.ReadInt32();
            if(br.BaseStream.Length != len)
            {
                _logger.Error("Received Invalid Message");
                throw new RosTopicException("Received Invalid Message");
            }
            data.Deserialize(br);
            return data;
        }
    }
}
