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
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Topic;
using RosSharp.Transport;
using RosSharp.Utility;

namespace RosSharp.Service
{
    internal sealed class ServiceProxyFactory
    {
        private readonly ILog _logger;

        public ServiceProxyFactory(string nodeId)
        {
            NodeId = nodeId;
            _logger = RosOutLogManager.GetCurrentNodeLogger(NodeId);
        }

        public string NodeId { get; private set; }

        public async Task<ServiceProxy<TService>> CreateAsync<TService>(string serviceName, Uri uri)
            where TService : IService, new()
        {
            var tcpClient = new TcpRosClient();

            var tcs = new TaskCompletionSource<ServiceProxy<TService>>();

            await tcpClient.ConnectAsync(uri.Host, uri.Port);
            var result = await ConnectToServiceServer<TService>(serviceName, tcpClient);
            return result;
        }

        private async Task<ServiceProxy<TService>> ConnectToServiceServer<TService>(string serviceName, TcpRosClient tcpClient)
            where TService : IService, new()
        {
            var receiveHeaderObs = tcpClient.ReceiveAsync()
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .Take(1)
                .PublishLast();

            receiveHeaderObs.Connect();

            var service = new TService();

            var sendHeader = new
            {
                callerid = NodeId,
                md5sum = service.Md5Sum,
                service = serviceName
            };

            var sendHeaderStream = new MemoryStream();

            TcpRosHeaderSerializer.Serialize(sendHeaderStream, sendHeader);

            try
            {
                var result = await tcpClient.SendAsync(sendHeaderStream.ToArray());
                var dummy = new TService();
                dynamic recvHeader = receiveHeaderObs.Timeout(TimeSpan.FromMilliseconds(Ros.TopicTimeout)).Wait();

                if (recvHeader.HasMember("service") && recvHeader.service != serviceName)
                {
                    _logger.Error(m => m("ServiceName mismatch error, expected={0} actual={1}", serviceName, recvHeader.topic));
                    throw new RosTopicException("ServiceName mismatch error");
                }
                if (recvHeader.md5sum != "*" && recvHeader.md5sum != dummy.Md5Sum)
                {
                    _logger.Error(m => m("MD5Sum mismatch error, expected={0} actual={1}", dummy.Md5Sum, recvHeader.md5sum));
                    throw new RosTopicException("MD5Sum mismatch error");
                }

                var proxy = new ServiceProxy<TService>(NodeId, serviceName, service, tcpClient);
                return proxy;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}