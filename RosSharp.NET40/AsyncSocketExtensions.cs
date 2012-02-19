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

namespace RosSharp
{
    public static class AsyncSocketExtensions
    {
        public static IObservable<SocketAsyncEventArgs> ConnectAsObservable(this Socket socket, EndPoint endpoint)
        {
            var socketEventArg = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };

            return Observable.Create<SocketAsyncEventArgs>(observer =>
            {
                var disposable = Observable.FromEventPattern<SocketAsyncEventArgs>(
                    e => socketEventArg.Completed += e, e => socketEventArg.Completed -= e)
                    .Select(e => e.EventArgs)
                    .Where(args => args.LastOperation == SocketAsyncOperation.Connect)
                    .Do(x =>
                    {
                        if (x.SocketError != SocketError.Success)
                        {
                            socket.Close();
                            throw new Exception();
                        }
                    })
                    .Take(1)
                    .Subscribe(observer);

                socket.ConnectAsync(socketEventArg);

                return disposable;
            });

        }

        public static IObservable<SocketAsyncEventArgs> SendAsObservable(this Socket socket, byte[] data)
        {
            var arg = new SocketAsyncEventArgs
            {
                RemoteEndPoint = socket.RemoteEndPoint,
                UserToken = null
            };
            arg.SetBuffer(data, 0, data.Length);

            return Observable.Create<SocketAsyncEventArgs>(observer =>
            {
                var disposable = Observable.FromEventPattern<SocketAsyncEventArgs>(
                    ev => arg.Completed += ev, ev => arg.Completed -= ev)
                    .Select(e => e.EventArgs)
                    .Where(args => args.LastOperation == SocketAsyncOperation.Send)
                    .Do(x =>
                    {
                        if (x.SocketError != SocketError.Success)
                        {
                            socket.Close();
                            throw new Exception();
                        }
                    })
                    .Take(1)
                    .Subscribe(observer);

                if (socket.Connected)
                {
                    socket.SendAsync(arg);
                }

                return disposable;
            });
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
                        if (x.SocketError != SocketError.Success)
                        {
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

#if !SILVERLIGHT && !WINDOWS_PHONE
        public static IObservable<SocketAsyncEventArgs> AcceptAsObservable(this Socket socket, EndPoint endpoint)
        {
            var arg = new SocketAsyncEventArgs
            {
                AcceptSocket = null
            };

            return Observable.Create<SocketAsyncEventArgs>(observer =>
            {
                var disposable = Observable.FromEventPattern<SocketAsyncEventArgs>(
                    e => arg.Completed += e, e => arg.Completed -= e)
                    .Select(e => e.EventArgs)
                    .Where(args => args.LastOperation == SocketAsyncOperation.Accept)
                    .Do(x =>
                    {
                        if (x.SocketError != SocketError.Success)
                        {
                            socket.Close();
                            throw new Exception();
                        }
                    })
                    .Subscribe(observer);

                socket.AcceptAsync(arg);

                return disposable;
            });

        }

#endif

    }
}
