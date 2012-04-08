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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace RosSharp.Transport
{
    internal static class AsyncSocketExtensions
    {
        private static ILog _logger = LogManager.GetCurrentClassLogger();

        public static Task ConnectTaskAsync(this Socket socket, EndPoint endpoint)
        {
            return Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, endpoint, null);
        }

        public static Task<int> SendTaskAsync(this Socket socket, byte[] data)
        {
            return Task<int>.Factory.FromAsync(
                (callback, o) => socket.BeginSend(data, 0, data.Length, SocketFlags.None, callback, o),
                socket.EndSend, null);
        }

        public static IObservable<SocketAsyncEventArgs> ReceiveAsObservable(this Socket socket)
        {
            var arg = new SocketAsyncEventArgs();
            arg.SetBuffer(new byte[1024], 0, 1024);

            return Observable.Create<SocketAsyncEventArgs>(observer =>
            {
                var disposable = Observable.FromEventPattern<SocketAsyncEventArgs>(
                    ev => arg.Completed += ev, ev => arg.Completed -= ev)
                    .Select(e => e.EventArgs)
                    .Where(args => args.LastOperation == SocketAsyncOperation.Receive)
                    .Do(x =>
                    {
                        _logger.Debug(m => m("Received: Code={0}", x.SocketError));
                        if (x.SocketError != SocketError.Success)
                        {
                            _logger.Debug(m => m("Close Socket[{0}]", socket.LocalEndPoint));
                            socket.Close();
                            throw new Exception();
                        }
                        if (socket.Connected)
                        {
                            socket.ReceiveAsync(arg);
                        }
                    })
                    .Subscribe(observer);

                if (socket.Connected)
                {
                    socket.ReceiveAsync(arg);
                }

                return disposable;
            });
        }

        public static IObservable<Socket> AcceptAsObservable(this Socket socket, EndPoint endpoint)
        {
            return Observable.Create<Socket>(observer =>
            {
                var disposable = Observable.Defer(
                    () => Observable.FromAsyncPattern<Socket>(socket.BeginAccept, socket.EndAccept).Invoke())
                    .Repeat()
                    .Subscribe(observer);
                return disposable;
            });
        }

        public static string Dump(this byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                if (b < 16) sb.Append('0');
                sb.Append(Convert.ToString(b, 16));
            }
            return sb.ToString();
        }
    }
}