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
    public class TcpClient
    {
        private Socket _socket;

        public IObservable<EventPattern<SocketAsyncEventArgs>> Connect(string hostName, int portNumber)
        {
            var hostEntry = new DnsEndPoint(hostName, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var socketEventArg = new SocketAsyncEventArgs {RemoteEndPoint = hostEntry};

            var obs = Observable.FromEventPattern<SocketAsyncEventArgs>(
                e => socketEventArg.Completed += e,
                e => socketEventArg.Completed -= e)
                .Take(1)
                .PublishLast();

            obs.Connect();

            _socket.ConnectAsync(socketEventArg);

            return obs;
        }

        public IObservable<SocketAsyncEventArgs> Send()
        {
            var serializer = new TcpRosHeaderSerializer<SubscriberHeader>();
            var msg = new StdMsgs.String() { Data = "test" };
            
            var header = new SubscriberHeader()
            {
                callerid = "test",
                topic = "/chatter",
                md5sum = msg.Md5Sum,
                type = msg.DataType
            };
            
            var ms = new MemoryStream();

            serializer.Serialize(ms, header);
            var data = ms.ToArray();

            var arg = new SocketAsyncEventArgs {
                RemoteEndPoint = _socket.RemoteEndPoint,
                UserToken = null
            };
            arg.SetBuffer(data, 0, data.Length);

            var sockAsObservable =  Observable.FromEventPattern<SocketAsyncEventArgs>(
                    ev => arg.Completed += ev, ev => arg.Completed -= ev)
                .Select(e => e.EventArgs)
                .Where(args => args.LastOperation == SocketAsyncOperation.Send);
            _socket.SendAsync(arg);

            return sockAsObservable;
        }

        public IObservable<byte[]> Receive()
        {
            var arg = new SocketAsyncEventArgs();
            arg.SetBuffer(new byte[1024], 0, 1024);

            var messages = new Subject<byte[]>();

            
                Observable.FromEventPattern<SocketAsyncEventArgs>(
                    ev => arg.Completed += ev, ev => arg.Completed -= ev)
                .Select(e => e.EventArgs)
                .Where(args => args.LastOperation == SocketAsyncOperation.Receive)
                .Select(OnReceive)
                .Scan(new byte[] { }, (abs, bs) =>
                    {
                        var rest = AppendData(abs, bs);
                        byte[] current;
                        if (CompleteMessage(out current, ref rest))
                        {
                            messages.OnNext(current);
                        }

                        if (_socket.Connected)
                        {
                            _socket.ReceiveAsync(arg);
                        }
                        return rest;
                    })
                    .Subscribe();

            if (_socket.Connected)
            {
                _socket.ReceiveAsync(arg);
            }

            return messages;
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
