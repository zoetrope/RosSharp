using System;
using System.IO;
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
using RosSharp.Transport;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class RosTopicServerTest
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
            var str = new std_msgs.String(){data = "test message"};
            var header = new
            {
                callerid = "test",
                topic = "mytopic",
                md5sum = str.Md5Sum,
                type = str.MessageType
            };
            var headerStream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(headerStream, header);

            var dataStream = new MemoryStream();
            var bw = new BinaryWriter(dataStream);
            bw.Write(str.SerializeLength);
            str.Serialize(bw);

            MTcpRosClient.AllInstances.ConnectAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            
            // 1.Receive Header, 2.Receive Data(std_msgs.String)
            int count = 0;
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => {
                if(count == 0)
                {
                    count++;
                    return Observable.Return(headerStream.ToArray());
                }
                else
                {
                    return Observable.Return(dataStream.ToArray());
                }
            };

            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task<int>.Factory.StartNew(() => t2.Length);

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() { HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS" });

            var subscriber = task.Result;

            var rec = subscriber.Timeout(TimeSpan.FromSeconds(3)).First();
            rec.data.Is("test message");
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ConnectionError()
        {
            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() {HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS"});

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof (SocketException));
        }


        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_SendAsyncError()
        {
            MTcpRosClient.AllInstances.ConnectAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(new byte[0]);

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() { HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS" });

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof(ArgumentException));
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_SendError()
        {
            MTcpRosClient.AllInstances.ConnectAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(new byte[0]);
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task<int>.Factory.StartNew(() => { throw new InvalidOperationException("Send Error"); });

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() { HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS" });

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof(InvalidOperationException));
            ex.InnerException.Message.Is("Send Error");

        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_HeaderDeserializeError()
        {
            MTcpRosClient.AllInstances.ConnectAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(new byte[0]);
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task<int>.Factory.StartNew(() => t2.Length);

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() { HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS" });

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof(RosTopicException));
            ex.InnerException.Message.Is("Stream length is too short");
        }

        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveHeaderTimeoutError()
        {
            Ros.TopicTimeout = 100;

            MTcpRosClient.AllInstances.ConnectAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(new byte[0]).Delay(TimeSpan.FromSeconds(3));
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task<int>.Factory.StartNew(() => t2.Length);

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() {HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS"});

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof (TimeoutException));
        }


        [TestMethod]
        [HostType("Moles")]
        public void StartAsync_ReceiveHeaderMismatch()
        {
            var str = new std_msgs.String();
            var header = new
            {
                callerid = "test",
                topic = "mytopic",
                md5sum = "aaaaaaaaaaaaa",
                type = str.MessageType
            };
            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, header);


            MTcpRosClient.AllInstances.ConnectAsyncStringInt32 = (t1, t2, t3) => Task.Factory.StartNew(() => { });
            MTcpRosClient.AllInstances.ReceiveAsyncInt32 = (t1, t2) => Observable.Return(stream.ToArray());
            MTcpRosClient.AllInstances.SendAsyncByteArray = (t1, t2) => Task<int>.Factory.StartNew(() => t2.Length);

            var server = new RosTopicServer<std_msgs.String>("mynode", "mytopic", new Uri("http://localhost"));

            var task = server.StartAsync(new TopicParam() { HostName = "test", PortNumber = 1234, ProtocolName = "TCPROS" });

            var ex = AssertEx.Throws<AggregateException>(() => task.Wait());
            ex.InnerException.GetType().Is(typeof(RosTopicException));
            ex.InnerException.Message.Is("MD5Sum mismatch error");
        }
    }
}
