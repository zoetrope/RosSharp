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
using System.Text;
using RosSharp.Transport;

namespace RosSharp.Service
{
    internal sealed class ServiceInstance<TService>
        where TService : IService, new()
    {
        private readonly string _nodeId;
        private readonly IService _service;
        private readonly TcpRosClient _client;

        public ServiceInstance(string nodeId, IService service, Socket s)
        {
            _nodeId = nodeId;
            _service = service;

            _client = new TcpRosClient(s);
        }

        internal void Initialize(string serviceName) //TODO: 非同期に。
        {
            _client.ReceiveAsync()
                .Take(1)
                .Select(b => TcpRosHeaderSerializer.Deserialize(new MemoryStream(b)))
                .SelectMany(_client.ReceiveAsync())
                .Subscribe(b =>
                {
                    var res = Invoke(new MemoryStream(b));
                    var array = res.ToArray();
                    _client.SendTaskAsync(array).Wait(); //TODO: Waitしても意味なくね？Subscribe自体の終了を待たねば。
                });

            var dummy = new TService();
            var header = new
            {
                callerid = _nodeId,
                md5sum = dummy.Md5Sum,
                service = serviceName,
                type = dummy.ServiceType
            };

            var ms = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(ms, header);
            _client.SendTaskAsync(ms.ToArray()).Wait();
        }

        private MemoryStream Invoke(Stream stream)
        {
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