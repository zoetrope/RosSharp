using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Transport;

namespace RosSharp.Tests.Transport
{
    [TestClass]
    public class TcpRosClientTest : ReactiveTest
    {
        [TestMethod]
        [HostType("Moles")]
        public void ConnectAsObservable_Success()
        {
            var arg  = new SocketAsyncEventArgs();

            var task = new Task(() => { });

            MAsyncSocketExtensions.ConnectTaskAsyncSocketEndPoint = (t1, t2) => task;
            
            var client = new TcpRosClient();

            client.ConnectAsync("127.0.0.1", 50000).ContinueWith(t => { });

            task.Start();
        }

        [TestMethod]
        [HostType("Moles")]
        public void ReceiveAsObservable_Success()
        {

            var data = new byte[] { 179, 0, 0, 0, 40, 0, 0, 0, 99, 97, 108, 108, 101, 114, 105, 100, 
                61, 47, 114, 111, 115, 106, 97, 118, 97, 95, 116, 117, 116, 111, 114, 105, 97, 108, 
                95, 112, 117, 98, 115, 117, 98, 47, 116, 97, 108, 107, 101, 114, 14, 0, 0, 0, 116, 
                111, 112, 105, 99, 61, 47, 99, 104, 97, 116, 116, 101, 114, 39, 0, 0, 0, 109, 100, 
                53, 115, 117, 109, 61, 57, 57, 50, 99, 101, 56, 97, 49, 54, 56, 55, 99, 101, 99, 
                56, 99, 56, 98, 100, 56, 56, 51, 101, 99, 55, 51, 99, 97, 52, 49, 100, 49, 20, 0, 
                0, 0, 116, 121, 112, 101, 61, 115, 116, 100, 95, 109, 115, 103, 115, 47, 83, 116, 
                114, 105, 110, 103, 32, 0, 0, 0, 109, 101, 115, 115, 97, 103, 101, 95, 100, 101, 102,
                105, 110, 105, 116, 105, 111, 110, 61, 115, 116, 114, 105, 110, 103, 32, 100, 97, 116,
                97, 10, 10, 10, 0, 0, 0, 108, 97, 116, 99, 104, 105, 110, 103, 61, 48 };

            var scheduler = new TestScheduler();

            MAsyncSocketExtensions.ReceiveAsObservableSocketIScheduler =
                (t1, t2) => scheduler.CreateHotObservable(OnNext(10, data));


            var observer = scheduler.CreateObserver<TcpRosHeader>();

            var client = new TcpRosClient();


            var result = client.ReceiveAsync()
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .Subscribe(observer);

            scheduler.AdvanceTo(10);

            var header = new 
            {
                callerid = "/rosjava_tutorial_pubsub/talker",
                latching = "0",
                md5sum = "992ce8a1687cec8c8bd883ec73ca41d1",
                message_definition = "string data\n\n",
                topic = "/chatter",
                type = "std_msgs/String",
            };

            observer.Messages.Count.Is(1);

            dynamic actual = observer.Messages.First().Value.Value;
            actual.callerid.Is(header.callerid);
            actual.latching.Is(header.latching);
            actual.md5sum.Is(header.md5sum);
            actual.message_definition.Is(header.message_definition);
            actual.topic.Is(header.topic);
            actual.type.Is(header.type);
        }


    }
}
