using System;
using System.Reactive.Subjects;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Topic;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class SubscriberTest : ReactiveTest
    {
        [TestMethod]
        [HostType("Moles")]
        public void Subscribe_Success()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<std_msgs.String>();
            var publisher = scheduler.CreateHotObservable(OnNext(10, new std_msgs.String() {data = "test data"}));

            MSlaveClient.ConstructorUri = (t1, t2) => { };
            MSlaveClient.AllInstances.RequestTopicAsyncStringStringListOfProtocolInfo =
                (t1, t2, t3, t4) => Task.Factory.StartNew(() => new TopicParam() {HostName = "localhost", PortNumber = 12345, ProtocolName = "TCPROS"});
            MRosTopicServer<std_msgs.String>.ConstructorStringStringUri = (t1, t2, t3, t4) => { };
            MRosTopicServer<std_msgs.String>.AllInstances.StartAsyncTopicParamBoolean =
                (t1, t2, t3) => Task.Factory.StartNew(() => (IObservable<std_msgs.String>)publisher);

            var sub = new Subscriber<std_msgs.String>("testtopic", "test");

            (sub as ISubscriber).UpdatePublishers(new List<Uri>() {new Uri("http://localhosst")});

            sub.Subscribe(observer);
            
            scheduler.AdvanceBy(100);

            observer.Messages.First().Value.Value.data.Is("test data");
        }
    }
}
