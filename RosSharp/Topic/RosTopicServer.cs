using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;
using System.Net.Sockets;

namespace RosSharp.Topic
{
    internal sealed class RosTopicServer<TDataType> : IDisposable
        where TDataType : IMessage, new()
    {
        private RosTcpClient _tcpClient;

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

        //TODO: いろいろ見直したい。
        public void Start(TopicParam param, Subject<TDataType> subject)
        {

            var tcpClient = new RosTcpClient();
            tcpClient.ConnectTaskAsync(param.HostName, param.PortNumber).Wait();

            var last = tcpClient.ReceiveAsync()
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
                .Subscribe(subject);

        }
    }
}
