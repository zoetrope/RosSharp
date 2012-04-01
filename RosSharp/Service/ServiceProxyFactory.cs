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
using System.Reactive.Linq;
using System.Text;
using RosSharp.Master;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Service
{
    internal sealed class ServiceProxyFactory
    {
        public ServiceProxyFactory(string nodeId)
        {
            NodeId = nodeId;
        }

        public string NodeId { get; private set; }

        public TService Create<TService>(string serviceName, Uri uri) //TODO: 非同期にしなくては。
            where TService : IService, new()
        {
            var tcpClient = new TcpRosClient();
            tcpClient.ConnectTaskAsync(uri.Host, uri.Port).Wait(); //TODO: waitではなくcontinueWith

            var rec = tcpClient.ReceiveAsync()
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .Take(1)
                .PublishLast();

            rec.Connect();

            var service = new TService();

            var header = new
            {
                callerid = NodeId,
                md5sum = service.Md5Sum,
                service = serviceName
            };

            var stream = new MemoryStream();

            TcpRosHeaderSerializer.Serialize(stream, header);
            var data = stream.ToArray();

            tcpClient.SendTaskAsync(data).Wait();

            var test = rec.First();

            //TODO: proxyは返さない？どっかに持っておかなくてよい？
            var proxy = new ServiceProxy<TService>(service, tcpClient);

            return service;
        }
    }
}