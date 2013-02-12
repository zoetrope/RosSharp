using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;

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
                new object[]
                {
                    1,
                    "/rosout",
                    "o",
                    "TCPROS",
                    "/rosout",
                    true
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetBusInfoStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetBusInfoIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            var info = client.GetBusInfoAsync("/test").Result;

            info.ConnectionId.Is(1);
            info.DestinationId.Is("/rosout");
            info.Direction.Is("o");
            info.Transport.Is("TCPROS");
            info.Topic.Is("/rosout");
            info.Connected.Is(true);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetBusInfo_Empty()
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

            //client.GetBusInfoAsync("/test").Result.Is(25346);
        }

        [TestMethod]
        [Ignore]
        [HostType("Moles")]
        public void GetBusStats_SuccessRosCppListener()
        {
            //意味不明。
            var result = new object[2][][]
            {
                new object[1][]
                {
                    new object[]
                    {
                        "/rosout",
                        new int[1][]
                        {
                            new int[5]
                            {
                                0,
                                483955,
                                483955,
                                2288,
                                0
                            }
                        }
                    }
                },
                new object[1][]
                {
                    new object[]
                    {
                        "/chatter",
                        new int[1][]
                        {
                            new int[5]
                            {
                                1,
                                44659,
                                2288,
                                0,
                                0
                            }
                        }
                    }
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetBusStatsStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetBusStatsIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            //client.GetBusInfoAsync("/test").Result.Is(25346);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetBusStats_SuccessRosPyListener()
        {

            var result = new object[3]
            {
                1,
                "",
                new object[3]
                {
                    new object[1][]
                    {
                        new object[3]
                        {
                            "/rosout",
                            453259,
                            new object[1][]
                            {
                                new object[4]
                                {
                                    1,
                                    454488,
                                    2887,
                                    true
                                }
                            }
                        }
                    },
                    new object[1][]
                    {
                        new object[2]
                        {
                            "/chatter",
                            new object[1][]
                            {
                                new object[5]
                                {
                                    2,
                                    183,
                                    4037,
                                    -1,
                                    true
                                }
                            }
                        }
                    },
                    new object[0]
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetBusStatsStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetBusStatsIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            var stats = client.GetBusStatsAsync("/test").Result;
            stats.PublishStatistics.Count.Is(1);
            stats.SubscribeStatistics.Count.Is(1);
            stats.ServiceStatistics.Count.Is(0);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetBusStats_Unknown()
        {
            var result = new object[3]
            {
                -1,
                "unknown node [/test]",
                ""
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetBusStatsStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MSlaveProxy.AllInstances.EndGetBusStatsIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));
            
            var ex = AssertEx.Throws<AggregateException>(() => client.GetBusStatsAsync("/test").Wait());
            ex.InnerException.Message.Is("unknown node [/test]");
            
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

            client.RequestTopicAsync("/test", "/rosout", new List<ProtocolInfo> { new ProtocolInfo(ProtocolType.TCPROS) }).Wait();

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

            client.RequestTopicAsync("/test", "/rosout", new List<ProtocolInfo> { new ProtocolInfo (ProtocolType.UDPROS)}).Wait();
            

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

            client.RequestTopicAsync("/test", "/chatter", new List<ProtocolInfo> { new ProtocolInfo(ProtocolType.TCPROS) }).Wait();

        }

    }
}
