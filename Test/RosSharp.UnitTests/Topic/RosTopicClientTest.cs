using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class RosTopicClientTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Ros.TopicTimeout = 2000;
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_Success()
        {
            var str = new std_msgs.String();
            var header = new
            {
                callerid = "test",
                topic = "mytopic",
                md5sum = str.Md5Sum,
                type = str.MessageType
            };
            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream,header);

            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task.Factory.StartNew(() => t2.Length);

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode","mytopic");

            topic.Connected.Is(false);
            topic.StartAsync(sock).Wait();
            topic.Connected.Is(true);
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveError()
        {
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => { throw new InvalidOperationException("Receive Error"); };

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<InvalidOperationException>(() => topic.StartAsync(sock).Wait());
            //ex.InnerException.GetType().Is(typeof(InvalidOperationException));
            ex.Message.Is("Receive Error");
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveHeaderTopicNameError()
        {
            var str = new std_msgs.String();
            var header = new
            {
                callerid = "test",
                topic = "unknown topic",
                md5sum = str.Md5Sum,
                type = str.MessageType
            };
            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, header);

            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<AggregateException>(() => topic.StartAsync(sock).Wait());
            ex.InnerException.GetType().Is(typeof(RosTopicException));
            ex.InnerException.Message.Is("TopicName mismatch error");
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveHeaderNotEnoughtMemberError()
        {
            var str = new std_msgs.String();
            var header = new
            {
                callerid = "test",
                type = str.MessageType
            };
            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, header);

            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<AggregateException>(() => topic.StartAsync(sock).Wait());
            ex.InnerException.GetType().Is(typeof(RuntimeBinderException));
        }
        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveTimeoutError()
        {
            Ros.TopicTimeout = 100;

            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => 
                Observable.Return(new byte[0]).Delay(TimeSpan.FromSeconds(3));

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<AggregateException>(() => topic.StartAsync(sock).Wait());
            ex.InnerException.GetType().Is(typeof(TimeoutException));
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_SendHeaderError()
        {
            var str = new std_msgs.String();
            var header = new
            {
                callerid = "test",
                topic = "mytopic",
                md5sum = str.Md5Sum,
                type = str.MessageType
            };
            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, header);

            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) =>{throw new InvalidOperationException("Send Error");};

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<AggregateException>(() => topic.StartAsync(sock).Wait());
            ex.InnerException.GetType().Is(typeof(InvalidOperationException));
            ex.InnerException.Message.Is("Send Error");
        }
        
        [TestMethod]
        [HostType("Moles")]
        public void SendTaskAsync_Success()
        {
            var str = new std_msgs.String();
            var header = new
            {
                callerid = "test",
                topic = "mytopic",
                md5sum = str.Md5Sum,
                type = str.MessageType
            };
            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, header);

            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task.Factory.StartNew(() => t2.Length);

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            topic.StartAsync(sock).Wait();
            topic.SendAsync(new std_msgs.String() {data = "test"}).Result.Is(12);
        }


        [TestMethod]
        [HostType("Moles")]
        public void SendTaskAsync_NotConnectedError()
        {
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");
            var ex = AssertEx.Throws<InvalidOperationException>(() => topic.SendAsync(new std_msgs.String() { data = "test" }).Wait());
            ex.Message.Is("Not Connected");
        }
    }
}
