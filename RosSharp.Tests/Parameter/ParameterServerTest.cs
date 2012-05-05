using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Parameter;
using RosSharp.Slave;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Tests.Parameter
{
    [TestClass]
    public class ParameterServerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Ros.HostName = "localhost";
        }

        [TestMethod]
        public void SetGet()
        {
            var server = new ParameterServer(new Uri("http://localhost"));

            server.HasParam("test", "test_param")[2].Is(false);

            server.SetParam("test", "test_param", 1234);

            server.HasParam("test", "test_param")[2].Is(true);

            server.GetParam("test", "test_param")[2].Is(1234);

        }

        [TestMethod]
        public void Subscribe()
        {
            var server = new ParameterServer(new Uri("http://localhost"));

            var slave = new SlaveServer("test", 0, new TopicContainer(), new TcpRosListener(0));
            var observer = new ReplaySubject<KeyValuePair<string, object>>();

            slave.ParameterUpdated += (key, value) => observer.OnNext(new KeyValuePair<string, object>(key, value));

            server.SetParam("test", "test_param", 1234);

            server.SubscribeParam("test", slave.SlaveUri.ToString(), "test_param");

            server.SetParam("test", "test_param", 5678);

            observer.First().Key.Is("test_param");
            observer.First().Value.Is(5678);
        }

        [TestMethod]
        public void SubscribeBeforeSetParam()
        {
            var server = new ParameterServer(new Uri("http://localhost"));

            var slave = new SlaveServer("test", 0, new TopicContainer(), new TcpRosListener(0));
            var observer = new ReplaySubject<KeyValuePair<string, object>>();

            slave.ParameterUpdated += (key, value) => observer.OnNext(new KeyValuePair<string, object>(key, value));

            server.SubscribeParam("test", slave.SlaveUri.ToString(), "test_param");

            server.SetParam("test", "test_param", 5678);

            observer.First().Key.Is("test_param");
            observer.First().Value.Is(5678);
        }


        [TestMethod]
        public void Unsubscribe()
        {
            var server = new ParameterServer(new Uri("http://localhost"));

            var slave = new SlaveServer("test", 0, new TopicContainer(), new TcpRosListener(0));
            var observer = new ReplaySubject<KeyValuePair<string, object>>();

            slave.ParameterUpdated += (key, value) => observer.OnNext(new KeyValuePair<string, object>(key, value));

            server.SetParam("test", "test_param", 1234);

            server.SubscribeParam("test", slave.SlaveUri.ToString(), "test_param");

            server.SetParam("test", "test_param", 5678);

            server.UnsubscribeParam("test", slave.SlaveUri.ToString(), "test_param");

            server.SetParam("test", "test_param", 9999);

            

        }
    }
}
