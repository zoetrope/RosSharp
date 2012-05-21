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
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using Common.Logging;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Service
{
    internal sealed class ServiceServer<TService> : IDisposable
        where TService : IService, new()
    {
        private TcpRosListener _listener;
        private readonly string _nodeId;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public ServiceServer(string nodeId)
        {
            _nodeId = nodeId;
        }

        public IPEndPoint EndPoint
        {
            get { return _listener.EndPoint; }
        }

        public IDisposable StartService(string serviceName, IService service)
        {
            _listener = new TcpRosListener(0);
            var disp = _listener.AcceptAsync()
                .Select(s => new ServiceInstance<TService>(_nodeId, service, s))
                .Subscribe(
                    client => client.StartAsync(serviceName),
                    ex =>
                    {
                        //TODO:
                        _logger.Error("Start Service Error", ex);
                    });

            return disp;
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}