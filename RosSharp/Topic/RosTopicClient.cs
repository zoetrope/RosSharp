#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Topic
{
    internal sealed class RosTopicClient<TMessage> : IDisposable
        where TMessage : IMessage, new()
    {
        private TcpRosClient _client;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public RosTopicClient(string nodeId, string topicName)
        {
            Connected = false;

            NodeId = nodeId;
            TopicName = topicName;
        }

        public string NodeId { get; private set; }
        public string TopicName { get; private set; }

        public bool Connected { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            _client.Dispose();
            Connected = false;
        }

        #endregion

        public Task<int> SendTaskAsync(TMessage data)
        {
            if (!Connected)
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


        public Task StartAsync(Socket socket, bool latching = false)
        {
            _client = new TcpRosClient(socket);

            return _client.ReceiveAsync()
                .Take(1)
                .Timeout(TimeSpan.FromMilliseconds(Ros.TopicTimeout))
                .Select(x => OnReceivedHeader(x, latching))
                .ToTask();
        }

        private Unit OnReceivedHeader(byte[] data, bool latching)
        {
            _logger.Debug("OnReceivedHeader");

            var dummy = new TMessage();
            dynamic reqHeader = TcpRosHeaderSerializer.Deserialize(new MemoryStream(data));

            if (reqHeader.topic != TopicName)
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

            if (reqHeader.HasMember("tcp_nodelay"))
            {
                _client.SetNodelayOption(reqHeader.tcp_nodelay == "1");
            }

            var resHeader = new
            {
                callerid = NodeId,
                latching = latching ? "1":"0",
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