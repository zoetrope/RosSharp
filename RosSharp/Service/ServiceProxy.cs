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
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;
using RosSharp.Transport;
using RosSharp.Utility;

namespace RosSharp.Service
{
    internal interface IServiceProxy : IAsyncDisposable
    {
        IMessage Invoke(IMessage request);
    }

    internal class ServiceProxy<TService> : IServiceProxy
        where TService : IService, new()
    {
        private readonly ILog _logger;
        private readonly TcpRosClient _client;

        public string NodeId { get; private set; }

        public ServiceProxy(string nodeId, string serviceName, TService service, TcpRosClient client)
        {
            NodeId = nodeId;
            _logger = RosOutLogManager.GetCurrentNodeLogger(NodeId);

            ServiceName = serviceName;
            Service = service;
            _client = client;

            Service.SetAction(Invoke);

            Service.Disposing += _ => DisposeAsync();
        }

        public TService Service { get; private set; }
        public string ServiceName { get; private set; }

        #region IServiceProxy Members

        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Disposing(ServiceName);
        }

        public IMessage Invoke(IMessage request)
        {
            var response = _client.ReceiveAsync(offset: 1)
                .Select(x =>
                {
                    var res = Service.CreateResponse();
                    var br = new BinaryReader(new MemoryStream(x));
                    br.ReadInt32();
                    res.Deserialize(br);
                    return res;
                })
                .Take(1)
                .PublishLast();

            response.Connect();

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(request.SerializeLength);
            request.Serialize(bw);
            var senddata = ms.ToArray();
            _client.SendAsync(senddata).Wait(TimeSpan.FromMilliseconds(Ros.TopicTimeout));

            return response.Timeout(TimeSpan.FromMilliseconds(Ros.TopicTimeout)).First();
        }

        #endregion
    }
}