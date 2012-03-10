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
        public  void TestTest()
        {
            Console.WriteLine(1);
            Debugger.Launch();
        }


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

            client.GetBusInfoAsync("/test").First().Is(25346);
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

            client.GetPidAsync("/test").First().Is(25346);
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

            var uri = client.GetMasterUriAsync("/test").First();
            uri.Is(new Uri("http://localhost:11311"));
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

            var topics = client.RequestTopicAsync("/test", "/rosout", new object[1] { new string[1] { "TCPROS" } }).First();


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

            var topics = client.RequestTopicAsync("/test", "/rosout", new object[1] { new string[1] { "UDPROS" } }).First();

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

            var topics = client.RequestTopicAsync("/test", "/chatter", new object[1] { new string[1] { "TCPROS" } }).First();

        }


        [TestMethod]
        [HostType("Moles")]
        public void GetPublications_Success()
        {
            var result = new object[3]
            {
                1,
                "success",
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

            var subs = client.GetPublicationsAsync("/test").First();

            subs.Count.Is(2);
            subs[0].Name.Is("/rosout");
            subs[0].Type.Is("rosgraph_msgs/Log");

            subs[1].Name.Is("/chatter");
            subs[1].Type.Is("std_msgs/String");
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetPublications_Empty()
        {
            var result = new object[3]
            {
                1,
                "success",
                new object[0]
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MSlaveProxy.AllInstances.BeginGetPublicationsStringAsyncCallbackObject = (t1, t2, t3, t4) =>{t3(null);return null;};
            MSlaveProxy.AllInstances.EndGetPublicationsIAsyncResult = (t1, t2) => result;

            var client = new SlaveClient(new Uri("http://localhost"));

            var subs = client.GetPublicationsAsync("/test").First();

            subs.Count.Is(0);
        }
    }
}
