using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace RosSharp
{
    public class RosTcpClient
    {
        private Socket _socket;

        
        public IObservable<SocketAsyncEventArgs> Connect(string hostName, int portNumber)
        {
            var hostEntry = new DnsEndPoint(hostName, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            return _socket.ConnectAsObservable(hostEntry);
        }

        public IObservable<SocketAsyncEventArgs> Send(byte[] data)
        {
            return _socket.SendAsObservable(data);
        }

        public IObservable<byte[]> ReceiveAsObservable()
        {
            return Observable.Create<byte[]>(observer =>
            {
                var disposable = _socket.ReceiveAsObservable()
                    .Select(OnReceive)
                    .Scan(new byte[] { }, (abs, bs) =>
                    {
                        var rest = AppendData(abs, bs);
                        byte[] current;
                        if (CompleteMessage(out current, ref rest))
                        {
                            observer.OnNext(current);
                        }

                        return rest;
                    })
                    .Subscribe();
                
                return disposable;
            });
        }


        protected byte[] OnReceive(SocketAsyncEventArgs args)
        {
            var ret = new byte[args.BytesTransferred];
            Buffer.BlockCopy(args.Buffer, 0, ret, 0, args.BytesTransferred);

            return ret;
        }

        protected byte[] AppendData(byte[] bs1, byte[] bs2)
        {
            var rs = new byte[bs1.Length + bs2.Length];
            bs1.CopyTo(rs, 0);
            bs2.CopyTo(rs, bs1.Length);
            return rs;
        }
        protected bool CompleteMessage(out byte[] current, ref byte[] rest)
        {
            if (rest.IsEmpty())
            {
                current = new byte[0];
                return false;
            }

            if (rest.Length < 4)
            {
                current = new byte[0];
                return false;
            }

            var length = BitConverter.ToInt32(rest, 0) + 4;

            if (rest.Length < length)
            {
                current = new byte[0];
                return false;
            }

            current = new byte[length];
            Buffer.BlockCopy(rest, 0, current, 0, length);

            var restLen = rest.Length - length;
            var temp = new byte[restLen];
            Buffer.BlockCopy(rest, length, rest, 0, restLen);
            rest = temp;

            return true;
        }
    }
}
