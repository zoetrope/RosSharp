using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Topic;
using RosSharp.Transport;
using RosSharp.Transport.Moles;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class RosTopicClientTest
    {
        [TestInitialize]
        public void Initialize()
        {
            ROS.TopicTimeout = 2000;
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

            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MRosTcpClient.AllInstances.SendTaskAsyncByteArray = (t1, t2) => Task.Factory.StartNew(() => t2.Length);

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode","mytopic");

            topic.Connected.Is(false);
            topic.StartAsync(sock).Timeout(TimeSpan.FromSeconds(3)).First();
            topic.Connected.Is(true);
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveError()
        {
            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => { throw new InvalidOperationException("Receive Error"); };

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<InvalidOperationException>(() => topic.StartAsync(sock).First());
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

            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<RosTopicException>(() => topic.StartAsync(sock).First());
            ex.Message.Is("TopicName mismatch error");
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

            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            AssertEx.Throws<RuntimeBinderException>(() => topic.StartAsync(sock).First());
        }
        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveTimeoutError()
        {
            ROS.TopicTimeout = 100;

            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => 
                Observable.Return(new byte[0]).Delay(TimeSpan.FromSeconds(3));

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            AssertEx.Throws<TimeoutException>(() => topic.StartAsync(sock).First());
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

            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MRosTcpClient.AllInstances.SendTaskAsyncByteArray = (t1, t2) =>{throw new InvalidOperationException("Send Error");};

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");

            var ex = AssertEx.Throws<InvalidOperationException>(() => topic.StartAsync(sock).First());
            ex.Message.Is("Send Error");
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

            MRosTcpClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MRosTcpClient.AllInstances.SendTaskAsyncByteArray = (t1, t2) => Task.Factory.StartNew(() => t2.Length);

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");
            
            topic.StartAsync(sock).Timeout(TimeSpan.FromSeconds(3)).First();
            topic.SendTaskAsync(new std_msgs.String() {data = "test"}).Result.Is(12);
        }


        [TestMethod]
        [HostType("Moles")]
        public void SendTaskAsync_NotConnectedError()
        {
            var topic = new RosTopicClient<std_msgs.String>("mynode", "mytopic");
            var ex = AssertEx.Throws<InvalidOperationException>(() => topic.SendTaskAsync(new std_msgs.String() { data = "test" }).Wait());
            ex.Message.Is("Not Connected");
        }
    }
}
