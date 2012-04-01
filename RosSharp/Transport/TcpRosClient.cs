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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common.Logging;

namespace RosSharp.Transport
{
    internal sealed class TcpRosClient : IDisposable
    {
        private Socket _socket;
        private BehaviorSubject<SocketAsyncEventArgs> _behaviorSubject;
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public TcpRosClient()
        {
        }

        public TcpRosClient(Socket socket)
        {
            _socket = socket;
        }

        public bool Connected
        {
            get { return _socket.Connected; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _socket.Close();
        }

        #endregion

        public Task ConnectTaskAsync(string hostName, int portNumber)
        {
            var hostEntry = new DnsEndPoint(hostName, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            return _socket.ConnectTaskAsync(hostEntry);
        }

        public Task<int> SendTaskAsync(byte[] data)
        {
            return _socket.SendTaskAsync(data);
        }

        public IObservable<byte[]> ReceiveAsync(int offset = 0)
        {
            if(_behaviorSubject ==null)
            {
                _behaviorSubject = new BehaviorSubject<SocketAsyncEventArgs>(null);
                _socket.ReceiveAsObservable().Subscribe(_behaviorSubject);
            }
            return Observable.Create<byte[]>(observer =>
            {
                var disposable = _behaviorSubject
                    .Where(x => x != null)
                    .Select(OnReceive)
                    .Scan(new byte[] {}, (abs, bs) =>
                    {
                        var rest = AppendData(abs, bs);

                        byte[] current;
                        if (CompleteMessage(offset, out current, ref rest))
                        {
                            _logger.Debug(m => m("OnNext = {0}", current.Dump()));
                            observer.OnNext(current);
                        }

                        return rest;
                    })
                    .Subscribe(_ => { },
                               ex =>
                               {
                                   Console.WriteLine("ReceiveAsync Error = {0}", ex.Message);
                                   observer.OnError(new Exception());
                                   observer.OnCompleted();
                               });

                return disposable;
            });
        }


        private byte[] OnReceive(SocketAsyncEventArgs args)
        {
            var ret = new byte[args.BytesTransferred];
            Buffer.BlockCopy(args.Buffer, 0, ret, 0, args.BytesTransferred);

            //_logger.Debug(m => m("rest = {0}", ret.Dump()));
            return ret;
        }

        private byte[] AppendData(byte[] bs1, byte[] bs2)
        {
            var rs = new byte[bs1.Length + bs2.Length];
            bs1.CopyTo(rs, 0);
            bs2.CopyTo(rs, bs1.Length);
            return rs;
        }

        private bool CompleteMessage(int offset, out byte[] current, ref byte[] rest)
        {
            if (rest.Length == 0)
            {
                current = new byte[0];
                return false;
            }

            if (rest.Length < 4 + offset)
            {
                current = new byte[0];
                return false;
            }


            var length = BitConverter.ToInt32(rest, offset) + 4;

            if (rest.Length < length + offset)
            {
                current = new byte[0];
                return false;
            }

            current = new byte[length];
            Buffer.BlockCopy(rest, offset, current, 0, length);

            var restLen = rest.Length - offset - length;
            var temp = new byte[restLen];
            Buffer.BlockCopy(rest, length, rest, 0, restLen);
            rest = temp;

            return true;
        }
    }
}