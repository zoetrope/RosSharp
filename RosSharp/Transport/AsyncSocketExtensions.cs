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
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Topic;

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

        public static IObservable<byte[]> ReceiveAsObservable(this Socket socket, IScheduler scheduler)
        {
            var arg = new SocketAsyncEventArgs();
            arg.SetBuffer(new byte[2048], 0, 2048);


            return Observable.Defer(
                () => Observable.Create<byte[]>(observer =>
                {
                    var read = Observable.FromAsyncPattern<byte[], int, int, SocketFlags, int>(socket.BeginReceive, socket.EndReceive);
                    var buffer = new byte[1024];
                    IDisposable disposable = read(buffer, 0, 1024, SocketFlags.None)
                        .Select(x =>
                        {
                            if (x == 0)
                            {
                                socket.Close();
                                throw new RosTopicException("Close Socket");
                            }
                            else
                            {
                                var ret = new byte[x];
                                Buffer.BlockCopy(buffer, 0, ret, 0, x);
                                return ret;
                            }
                        })
                        .Subscribe(observer);
                    return disposable;
                }))
                .Repeat();

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