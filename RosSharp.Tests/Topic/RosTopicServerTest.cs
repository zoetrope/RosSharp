using System;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Topic;
using RosSharp.Transport.Moles;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class RosTopicServerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var t = Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task => Console.WriteLine("Fail"), TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith(task => Console.WriteLine("Run"), TaskContinuationOptions.OnlyOnRanToCompletion);

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_Success()
        {
            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic");

            var task = server.StartAsync(new TopicParam() {HostName = "localhost", PortNumber = 1234, ProtocolName = "TCPROS"});

            var x = task.ContinueWith(
                t => Console.WriteLine("EEEEEEEEEEEE={0}", t.Exception),
                TaskContinuationOptions.OnlyOnFaulted);

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ConnectionError()
        {
            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic");

            var task = server.StartAsync(new TopicParam() {HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS"});

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof (SocketException));
        }


        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_SendError()
        {

            MRosTcpClient.AllInstances.ConnectTaskAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => { throw new InvalidOperationException("Receive Error"); };

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic");

            var task = server.StartAsync(new TopicParam() { HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS" });

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof(SocketException));
        }
    }
}
