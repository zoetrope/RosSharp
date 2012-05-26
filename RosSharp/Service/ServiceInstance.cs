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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Service
{
    internal sealed class ServiceInstance<TService>
        where TService : IService, new()
    {
        private readonly string _nodeId;
        private readonly IService _service;
        private readonly TcpRosClient _client;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private string _serviceName;

        public ServiceInstance(string nodeId, IService service, Socket s)
        {
            _nodeId = nodeId;
            _service = service;

            _client = new TcpRosClient(s);
        }

        internal Task StartAsync(string serviceName)
        {
            _serviceName = serviceName;

            return _client.ReceiveAsync()
                .Take(1)
                .Select(b => TcpRosHeaderSerializer.Deserialize(new MemoryStream(b)))
                .Select(x => ConnectToService(x))
                .ToTask();
        }

        private Task ConnectToService(dynamic header)
        {
            var dummy = new TService();
            _logger.Info(m => m("Receive Header {0}", header.ToString()));

            if (header.service != _serviceName)
            {
                _logger.Error(m => m("ServiceName mismatch error, expected={0} actual={1}", _serviceName, header.topic));
                throw new RosTopicException("ServiceName mismatch error");
            }
            if (header.md5sum != "*" && header.md5sum != dummy.Md5Sum)
            {
                _logger.Error(m => m("MD5Sum mismatch error, expected={0} actual={1}", dummy.Md5Sum, header.md5sum));
                throw new RosTopicException("MD5Sum mismatch error");
            }

            var sendHeader = new
            {
                callerid = _nodeId,
                md5sum = dummy.Md5Sum,
                service = _serviceName,
                type = dummy.ServiceType
            };

            var ms = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(ms, sendHeader);

            return _client.SendAsync(ms.ToArray())
                .ContinueWith(task =>
                {
                    _logger.Info("SendTaskAsync ContinueWith");

                    _client.ReceiveAsync()
                        .Subscribe(b =>
                        {
                            var res = Invoke(new MemoryStream(b));
                            var array = res.ToArray();
                            try
                            {
                                _client.SendAsync(array).Wait();
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Send Error", ex);
                            }

                        }, ex =>
                        {
                            _logger.Error("Receive Error", ex);
                        });

                });
        }

        private MemoryStream Invoke(Stream stream)
        {
            _logger.Info("Invoke!");

            var dummy = new TService();
            var req = dummy.CreateRequest();

            var br = new BinaryReader(stream);
            var len = br.ReadInt32();
            req.Deserialize(br);

            var res = _service.Invoke(req);

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write((byte) 1);
            bw.Write(res.SerializeLength);
            res.Serialize(bw);

            return ms;
        }
    }
}