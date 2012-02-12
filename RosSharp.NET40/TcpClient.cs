using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
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
                .Do(x => Console.WriteLine(x.EventArgs.LastOperation))
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

        public void Receive()
        {
            var arg = new SocketAsyncEventArgs();
            arg.SetBuffer(new byte[1024], 0, 1024);

            var socketAsObservable =
                Observable.FromEventPattern<SocketAsyncEventArgs>(
                    ev => arg.Completed += ev,ev => arg.Completed -= ev)
                .Select(e => e.EventArgs)
                .Where(args => args.LastOperation == SocketAsyncOperation.Receive)
                .Subscribe(OnReceive);

            if (_socket.Connected) _socket.ReceiveAsync(arg);
        }

        protected void OnReceive(SocketAsyncEventArgs args)
        {
            string data = Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);
            
            Array.Clear(args.Buffer, 0, 1024);

            if (data.Length > 0)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                Array.Copy(bytes, args.Buffer, bytes.Length);
                args.SetBuffer(bytes.Length, 1024 - bytes.Length);
            }
            else
                args.SetBuffer(0, 1024);

            if (_socket.Connected) _socket.ReceiveAsync(args);

        }
    }
}
