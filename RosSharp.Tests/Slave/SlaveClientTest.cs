using System;
using System.Diagnostics;
using System.Reactive.Linq;
using CookComputing.XmlRpc.Moles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Slave.Moles;

namespace RosSharp.Tests.Slave
{
    [TestClass]
    public class SlaveClientTest
    {
        [TestMethod]
        [HostType("Moles")]
        public void GetBusInfo_Success()
        {
            
            var result = new object[3]
            {
                1,
                "bus info",
                new object[0]
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetBusInfoStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetBusInfoIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.GetBusInfoAsync("/test").Result.Is(25346);
        }


        [TestMethod]
        [HostType("Moles")]
        public void GetPid_Success()
        {
            var result = new object[3]
            {
                1,
                "PID: 25346",
                25346
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetPidStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetPidIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.GetPidAsync("/test").Result.Is(25346);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetMasterUri_Success()
        {
            var result = new object[3]
            {
                1,
                "",
                "http://localhost:11311"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetMasterUriStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetMasterUriIAsyncResult= (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.GetMasterUriAsync("/test").Result.Is(new Uri("http://localhost:11311"));
        }



        [TestMethod]
        [HostType("Moles")]
        public void GetPublications_Success()
        {
            var result = new object[3]
            {
                1,
                "Success",
                new string[2][]{
                    new string[2]
                    {
                        "/rosout",
                        "rosgraph_msgs/Log"
                    },
                    new string[2]
                    {
                        "/chatter",
                        "std_msgs/String"
                    }
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetPublicationsStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetPublicationsIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            var ret = client.GetPublicationsAsync("/test").Result;

            ret.Count.Is(2);
            ret[0].TopicName.Is("/rosout");
            ret[0].MessageType.Is("rosgraph_msgs/Log");

            ret[1].TopicName.Is("/chatter");
            ret[1].MessageType.Is("std_msgs/String");
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetPublications_Empty()
        {
            var result = new object[3]
            {
                1,
                "Success",
                new object[0]
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetPublicationsStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetPublicationsIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.GetPublicationsAsync("/test").Result.Count.Is(0);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetSubscriptions_Success()
        {
            var result = new object[3]
            {
                1,
                "Success",
                new string[1][]{
                    new string[2]
                    {
                        "/chatter",
                        "std_msgs/String"
                    }
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetSubscriptionsStringAsyncCallbackObject= (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetSubscriptionsIAsyncResult= (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            var ret = client.GetSubscriptionsAsync("/test").Result;

            ret.Count.Is(1);
            ret[0].TopicName.Is("/chatter");
            ret[0].MessageType.Is("std_msgs/String");
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetSubscriptions_Empty()
        {
            var result = new object[3]
            {
                1,
                "Success",
                new object[0]
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetSubscriptionsStringAsyncCallbackObject= (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetSubscriptionsIAsyncResult= (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.GetSubscriptionsAsync("/test").Result.Count.Is(0);

        }

        [TestMethod]
        [HostType("Moles")]
        public void RequestTopic_Success()
        {
            var result = new object[3]
            {
                1,
                "Protocol<TCPROS, AdvertiseAddress<192.168.11.4, 38939>>",
                new object[3]{
                    "TCPROS",
                    "192.168.11.4",
                    38939
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginRequestTopicStringStringObjectArrayAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MSlaveProxy.AllInstances.EndRequestTopicIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.RequestTopicAsync("/test", "/rosout", new object[1] { new string[1] { "TCPROS" } }).Wait();

        }

        [TestMethod]
        [HostType("Moles")]
        public void PublisherUpdate_Success()
        {
            var result = new object[3]
            {
                1,
                "Publisher update received.",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginPublisherUpdateStringStringStringArrayAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MSlaveProxy.AllInstances.EndPublisherUpdateIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.PublisherUpdateAsync("/test", "topic", new string[1] { "http://192.168.1.2:8989" }).Wait();

        }
        [TestMethod]
        [HostType("Moles")]
        public void RequestTopic_NoSupportedProtocol()
        {
            var result = new object[3]
            {
                -1,
                "No supported protocols specified.",
                "null"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginRequestTopicStringStringObjectArrayAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MSlaveProxy.AllInstances.EndRequestTopicIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.RequestTopicAsync("/test", "/rosout", new object[1] { new string[1] { "UDPROS" } }).Wait();
            

        }

        [TestMethod]
        [HostType("Moles")]
        public void RequestTopic_NoPublisher()
        {
            var result = new object[3]
            {
                -1,
                "No publishers for topic: /chatter",
                "null"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginRequestTopicStringStringObjectArrayAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MSlaveProxy.AllInstances.EndRequestTopicIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            client.RequestTopicAsync("/test", "/chatter", new object[1] { new string[1] { "TCPROS" } }).Wait();

        }

    }
}
