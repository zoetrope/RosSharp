using System;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Transport;

namespace RosSharp.Tests.Transport
{
    [TestClass]
    public class TcpRosListenerTest : ReactiveTest
    {
        [TestMethod]
        [HostType("Moles")]
        public void AcceptAsync_Success()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var scheduler = new TestScheduler();

            MAsyncSocketExtensions.AcceptAsObservableSocketEndPoint =
                (t1, t2) => scheduler.CreateHotObservable(OnNext(10, sock));

            var observer = scheduler.CreateObserver<Socket>();

            var client = new TcpRosListener(0);

            var result = client.AcceptAsync().Subscribe(observer);

            scheduler.AdvanceTo(10);

            observer.Messages.Is(OnNext(10, sock));

        }

    }
}
