using System;
using System.Security.Policy;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Topic;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class TopicContainerTest
    {
        [TestMethod]
        public void AddPublisher_Success()
        {
            var container = new TopicContainer();
            IPublisher pub;

            container.HasPublisher("pub1").Is(false);
            container.GetPublishers().Count.Is(0);
            container.GetPublisher("pub1", out pub).Is(false);

            container.AddPublisher(new Publisher<std_msgs.String>("pub1", "test")).Is(true);
            container.HasPublisher("pub1").Is(true);
            container.GetPublishers().Count.Is(1);
            container.GetPublisher("pub1", out pub).Is(true);

            container.AddPublisher(new Publisher<std_msgs.String>("pub2", "test")).Is(true);
            container.HasPublisher("pub1").Is(true);
            container.HasPublisher("pub2").Is(true);
            container.GetPublishers().Count.Is(2);
            container.GetPublisher("pub2", out pub).Is(true);

        }

        [TestMethod]
        public void RemovePublisher_Success()
        {
            var container = new TopicContainer();
            IPublisher pub;

            container.HasPublisher("pub1").Is(false);
            container.GetPublishers().Count.Is(0);

            container.AddPublisher(new Publisher<std_msgs.String>("pub1", "test")).Is(true);
            container.HasPublisher("pub1").Is(true);
            container.GetPublishers().Count.Is(1);
            container.GetPublisher("pub1", out pub).Is(true);

            container.RemovePublisher("pub1").Is(true);
            container.GetPublisher("pub1", out pub).Is(false);

            container.RemovePublisher("pub1").Is(false);
        }


        [TestMethod]
        public void AddPublisher_AlreadyAdded()
        {
            var container = new TopicContainer();
            IPublisher pub;

            container.HasPublisher("pub1").Is(false);
            container.GetPublishers().Count.Is(0);

            container.AddPublisher(new Publisher<std_msgs.String>("pub1", "test")).Is(true);
            container.HasPublisher("pub1").Is(true);
            container.GetPublishers().Count.Is(1);
            container.GetPublisher("pub1", out pub).Is(true);

            container.AddPublisher(new Publisher<std_msgs.String>("pub1", "test")).Is(false);
            container.HasPublisher("pub1").Is(true);
            container.GetPublishers().Count.Is(1);
            container.GetPublisher("pub1", out pub).Is(true);

        }


        [TestMethod]
        public void AddSubscriber_Success()
        {
            var container = new TopicContainer();
            ISubscriber sub;

            container.HasSubscriber("sub1").Is(false);
            container.GetSubscribers().Count.Is(0);
            container.GetSubscriber("sub1", out sub).Is(false);

            container.AddSubscriber(new Subscriber<std_msgs.String>("sub1", "test")).Is(true);
            container.HasSubscriber("sub1").Is(true);
            container.GetSubscribers().Count.Is(1);
            container.GetSubscriber("sub1", out sub).Is(true);

            container.AddSubscriber(new Subscriber<std_msgs.String>("sub2", "test")).Is(true);
            container.HasSubscriber("sub1").Is(true);
            container.HasSubscriber("sub2").Is(true);
            container.GetSubscribers().Count.Is(2);
            container.GetSubscriber("sub2", out sub).Is(true);

        }

        [TestMethod]
        public void RemoveSubscriber_Success()
        {
            var container = new TopicContainer();
            ISubscriber sub;

            container.HasSubscriber("sub1").Is(false);
            container.GetSubscribers().Count.Is(0);

            container.AddSubscriber(new Subscriber<std_msgs.String>("sub1", "test")).Is(true);
            container.HasSubscriber("sub1").Is(true);
            container.GetSubscribers().Count.Is(1);
            container.GetSubscriber("sub1", out sub).Is(true);

            container.RemoveSubscriber("sub1").Is(true);
            container.GetSubscriber("sub1", out sub).Is(false);

            container.RemoveSubscriber("sub1").Is(false);
        }


        [TestMethod]
        public void AddSubscriber_AlreadyAdded()
        {
            var container = new TopicContainer();
            ISubscriber sub;

            container.HasSubscriber("sub1").Is(false);
            container.GetSubscribers().Count.Is(0);

            container.AddSubscriber(new Subscriber<std_msgs.String>("sub1", "test")).Is(true);
            container.HasSubscriber("sub1").Is(true);
            container.GetSubscribers().Count.Is(1);
            container.GetSubscriber("sub1", out sub).Is(true);

            container.AddSubscriber(new Subscriber<std_msgs.String>("sub1", "test")).Is(false);
            container.HasSubscriber("sub1").Is(true);
            container.GetSubscribers().Count.Is(1);
            container.GetSubscriber("sub1", out sub).Is(true);

        }
    }
}
