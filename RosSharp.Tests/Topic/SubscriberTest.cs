using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Topic;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class SubscriberTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sub = new Subscriber<std_msgs.String>("testtopic", "test");

            (sub as ISubscriber).UpdatePublishers(new List<Uri>() {new Uri("http://localhosst")});


        }
    }
}
