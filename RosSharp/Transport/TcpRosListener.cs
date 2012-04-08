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
using System.Net.Sockets;
using System.Reactive.Linq;
using Common.Logging;

namespace RosSharp.Transport
{
    internal sealed class TcpRosListener : IDisposable
    {
        private Socket _socket;
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public TcpRosListener(int portNumber)
        {
            var hostEntry = new IPEndPoint(IPAddress.Any, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(hostEntry);
            _socket.Listen(50);
        }

        public bool Connected
        {
            get { return _socket.Connected; }
        }

        public IPEndPoint EndPoint
        {
            get { return (IPEndPoint) _socket.LocalEndPoint; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _logger.Debug(m => m("Close Socket[{0}]", _socket.LocalEndPoint));
            _socket.Close();
            _socket = null;
        }

        #endregion

        public IObservable<Socket> AcceptAsync()
        {
            return Observable.Create<Socket>(
                observer =>
                {
                    
                        return _socket.AcceptAsObservable(_socket.LocalEndPoint).Subscribe(observer);
                    
                });
        }
    }
}