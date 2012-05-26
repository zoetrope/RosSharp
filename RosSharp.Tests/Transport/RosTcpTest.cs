using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Transport;

namespace RosSharp.Tests.Transport
{
    [TestClass]
    public class RosTcpTest
    {

        [TestMethod]
        public void Receive_Error()
        {
            var listener = new TcpRosListener(0);

            listener.AcceptAsync()
                .Do(_ => Console.WriteLine("Connected"))
                .Subscribe(s =>
                {
                    new TcpRosClient(s).ReceiveAsync()
                        .Subscribe(data => Console.WriteLine("Received:{0}", data),
                                   ex => Console.WriteLine("Received Error: {0}", ex.Message));
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    s.Close();//3秒待って閉じる。
                },
                           ex => Console.WriteLine("Receive Error = {0}", ex.Message));

            int port = listener.EndPoint.Port;

            var client3 = new TcpRosClient();

            client3.ConnectAsync("localhost", port).Wait();

            var sendData = new byte[] { 1, 0, 0, 0, 1 };
            client3.SendAsync(sendData).Wait();

            Thread.Sleep(TimeSpan.FromSeconds(5));

            try
            {
                client3.SendAsync(sendData).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send Error:{0}", ex.Message);
            }
        }

        [TestMethod]
        public void SendAndReceive()
        {
            var listener = new TcpRosListener(0);

            listener.AcceptAsync()
                .Do(_ => Console.WriteLine("Connected"))
                .Subscribe(s => new TcpRosClient(s).ReceiveAsync()
                                    .Subscribe(data => Console.WriteLine("Received:{0}", data)));

            int port = listener.EndPoint.Port;

            Console.WriteLine("Press Any Key 3");
            Console.ReadKey();

            var client3 = new TcpRosClient();

            client3.ConnectAsync("localhost", port).Wait();

            var sendData = new byte[] { 1, 0, 0, 0, 1 };
            client3.SendAsync(sendData).Wait();
        }

        [TestMethod]
        public void MultipleConnection()
        {
            var listener = new TcpRosListener(0);

            listener.AcceptAsync().Subscribe(
                x => Console.WriteLine("new socket = {0}", x.RemoteEndPoint),
                ex => Console.WriteLine("accept error = {0}", ex.Message));

            int port = listener.EndPoint.Port;
            var client1 = new TcpRosClient();
            client1.ConnectAsync("localhost", port)
                .ContinueWith(t => Console.WriteLine("task.ex={0}", t.Exception));
            var client2 = new TcpRosClient();
            client2.ConnectAsync("localhost", port)
                .ContinueWith(t => Console.WriteLine("task.ex={0}", t.Exception));
            var client3 = new TcpRosClient();
            client3.ConnectAsync("localhost", port)
                .ContinueWith(t => Console.WriteLine("task.ex={0}", t.Exception));
        }

        [TestMethod]
        public void DisposeError()
        {
            var listener = new TcpRosListener(0);

            listener.AcceptAsync().Subscribe(
                x => Console.WriteLine("new socket = {0}", x.RemoteEndPoint),
                ex => Console.WriteLine("accept error = {0}", ex.Message));


            int port = listener.EndPoint.Port;

            listener.Dispose();

            Console.WriteLine("Press Any Key 3");
            Console.ReadKey();

            var client3 = new TcpRosClient();

            try
            {
                client3.ConnectAsync("localhost", port).Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("agg={0}", ex.Message);
            }
        }
    }
}
