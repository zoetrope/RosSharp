using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
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
                    //.Do(x=>Console.WriteLine("last = {0}",x.EventArgs.LastOperation))
                    .Select(e => e.EventArgs)
                    .Where(args => args.LastOperation == SocketAsyncOperation.Receive)
                    .Do(x =>
                        {
                            _logger.Debug(m => m("Received: Error={0}", x.SocketError));
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


    }
}
