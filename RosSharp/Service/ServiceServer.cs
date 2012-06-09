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
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Transport;

namespace RosSharp.Service
{
    public interface IServiceServer : IAsyncDisposable
    {
        string ServiceName { get; }
    }

    internal sealed class ServiceServer<TService> : IServiceServer
        where TService : IService, new()
    {
        private TcpRosListener _listener;
        private readonly string _nodeId;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly CompositeDisposable _instanceDisposables = new CompositeDisposable();
        private IDisposable _disposable;

        public string ServiceName { get; private set; }
        public Task DisposeAsync()
        {
            _disposable.Dispose();
            _instanceDisposables.Dispose();
            _listener.Dispose();

            return Disposing(ServiceName);
        }

        public ServiceServer(string nodeId)
        {
            _nodeId = nodeId;
        }

        public IPEndPoint EndPoint
        {
            get { return _listener.EndPoint; }
        }

        public void StartService(string serviceName, IService service)
        {
            ServiceName = serviceName;

            _listener = new TcpRosListener(0);
            _disposable = _listener.AcceptAsync()
                .Select(socket => new ServiceInstance<TService>(_nodeId, service, socket))
                .Do(instance=>_instanceDisposables.Add(instance))
                .SelectMany(instance => instance.StartAsync(serviceName).ToObservable())
                .Subscribe(_ => { }, ex => _logger.Error("Start Service Error", ex));
        }

        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        public void Dispose()
        {
            DisposeAsync().Wait();
        }
    }
}