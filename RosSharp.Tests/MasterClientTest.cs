using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CookComputing.XmlRpc.Moles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.Master.Moles;
using RosSharp.Moles;

namespace RosSharp.Tests
{
    [TestClass]
    public class MasterClientTest
    {
        
        [TestMethod]
        [HostType("Moles")]
        public void RegisterService_Success()
        {
            var result = new object[3]
            {
                1,
                "Registered [/test] as provider of [/myservice]",
                1
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginRegisterServiceStringStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6, t7) => { t6(null); return null; };
            MMasterProxy.AllInstances.EndRegisterServiceIAsyncResult= (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            client.RegisterServiceAsync("/test", "/myservice", new Uri("http://192.168.11.2:11112"), new Uri("http://192.168.11.2:11111")).First();
        }
        [TestMethod]
        [HostType("Moles")]
        public void RegisterService_ParameterError()
        {
            var result = new object[3]
            {
                -1,
                "ERROR: parameter [service_api] is not an XMLRPC URI",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginRegisterServiceStringStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6, t7) => { t6(null); return null; };
            MMasterProxy.AllInstances.EndRegisterServiceIAsyncResult= (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<InvalidOperationException>(() => client.RegisterServiceAsync("/test", "/myservice", new Uri("http://localhost"), new Uri("http://localhost")).First());
            ex.Message.Is("ERROR: parameter [service_api] is not an XMLRPC URI");
        }
        [TestMethod]
        [HostType("Moles")]
        public void UnregisterService_Success()
        {
            var result = new object[3]
            {
                1,
                "Unregistered [/test] as provider of [/myservice]",
                1
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginUnregisterServiceStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MMasterProxy.AllInstances.EndUnregisterServiceIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ret = client.UnregisterServiceAsync("/test", "/myservice", new Uri("http://localhost")).First();
            ret.Is(1);
        }
        [TestMethod]
        [HostType("Moles")]
        public void UnregisterService_NotRegistered()
        {
            var result = new object[3]
            {
                1,
                "[/test] is not a registered node",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginUnregisterServiceStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MMasterProxy.AllInstances.EndUnregisterServiceIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ret = client.UnregisterServiceAsync("/test", "/myservice", new Uri("http://localhost")).First();
            ret.Is(0);
        }
        [TestMethod]
        [HostType("Moles")]
        public void UnregisterService_ParameterError()
        {
            var result = new object[3]
            {
                -1,
                "ERROR: parameter [service] must be a non-empty string",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginUnregisterServiceStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MMasterProxy.AllInstances.EndUnregisterServiceIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<InvalidOperationException>(() => client.UnregisterServiceAsync("/test", "/myservice", new Uri("http://localhost")).First());
            ex.Message.Is("ERROR: parameter [service] must be a non-empty string");
        }

        [TestMethod]
        [HostType("Moles")]
        public void RegisterSubscriber_Success()
        {
            var result = new object[3]
            {
                1,
                "Subscribed to [/topic1]",
                new object[0]
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginRegisterSubscriberStringStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6, t7) => { t6(null); return null; };
            MMasterProxy.AllInstances.EndRegisterSubscriberIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ret =  client.RegisterSubscriberAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11112")).First();
            
        }
        [TestMethod]
        [HostType("Moles")]
        public void RegisterSubscriber_ParameterError()
        {
            var result = new object[3]
            {
                -1,
                "ERROR: parameter [topic_type] is not a valid package resource name",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginRegisterSubscriberStringStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6, t7) => { t6(null); return null; };
            MMasterProxy.AllInstances.EndRegisterSubscriberIAsyncResult= (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<InvalidOperationException>(() => client.RegisterSubscriberAsync("/test", "topic1", "topicType", new Uri("http://192.168.11.2:11112")).First());
            ex.Message.Is("ERROR: parameter [topic_type] is not a valid package resource name");
        }


        [TestMethod]
        [HostType("Moles")]
        public void RegisterPublisher_Success()
        {
            var result = new object[3]
            {
                1,
                "Registered [/test] as publisher of [/topic1]",
                new string[1]{"http://192.168.11.2:11112/"}
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginRegisterPublisherStringStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6, t7) => { t6(null); return null; };
            MMasterProxy.AllInstances.EndRegisterPublisherIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ret = client.RegisterPublisherAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11113")).First();
            ret[0].Is(new Uri("http://192.168.11.2:11112/"));

        }

        [TestMethod]
        [HostType("Moles")]
        public void LookupNode_Success()
        {
            var result = new object[3]
            {
                1,
                "node api",
                "http://192.168.11.4:59511/"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginLookupNodeStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MMasterProxy.AllInstances.EndLookupNodeIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            client.LookupNodeAsync("/test", "/rosout").First().Is(new Uri("http://192.168.11.4:59511/"));

        }

        [TestMethod]
        [HostType("Moles")]
        public void LookupNode_UnknownError()
        {
            var result = new object[3]
            {
                -1,
                "unknown node [/hogehoge]",
                ""
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginLookupNodeStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MMasterProxy.AllInstances.EndLookupNodeIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<InvalidOperationException>(() => client.LookupNodeAsync("/test", "/hogehoge").First());
            ex.Message.Is("unknown node [/hogehoge]");
        }

        [TestMethod]
        [HostType("Moles")]
        public void LookupNode_ParameterError()
        {
            var result = new object[3]
            {
                -1,
                "ERROR: parameter [node] must be a non-empty string",
                ""
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginLookupNodeStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MMasterProxy.AllInstances.EndLookupNodeIAsyncResult= (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<InvalidOperationException>(() => client.LookupNodeAsync("/test", null).First());
            ex.Message.Is("ERROR: parameter [node] must be a non-empty string");
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetSystemState_Success()
        {
            var result = new object[3]
            {
                1,
                "current system state",
                new object[3][][]{
                    new object[3][]{
                        new object[2]{
                            "/chatter",
                            new string[1]{
                                "/rosjava_tutorial_pubsub/talker"
                            }
                        },
                        new object[2]{
                            "/rosout",
                            new string[2]{
                                "/rosjava_tutorial_pubsub/listener",
                                "/rosjava_tutorial_pubsub/talker"
                            }
                        },
                        new object[2]{
                            "/rosout_agg",
                            new string[1]{
                                "/rosout"
                            }
                        }
                    },
                    new object[2][]{
                        new object[2]{
                            "/chatter",
                            new string[1]{
                                "/rosjava_tutorial_pubsub/listener"
                            }
                        },
                        new object[2]{
                            "/rosout",
                            new string[1]{
                                "/rosout"
                            }
                        }
                    },
                    new object[2][]{
                        new object[2]{
                            "/rosout/set_logger_level",
                            new string[1]{
                                "/rosout"
                            }
                        },
                        new object[2]{
                            "/rosout/get_loggers",
                            new string[1]{
                                "/rosout"
                            }
                        }
                    }
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginGetSystemStateStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MMasterProxy.AllInstances.EndGetSystemStateIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var state = client.GetSystemStateAsync("/test").First();
            state.Publishers.Count.Is(3);
            state.Subscribers.Count.Is(2);
            state.Services.Count.Is(2);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetSystemState_ParameterError()
        {
            var result = new object[3]
                        {
                            -1,
                            "caller_id must be a string",
                            new object[3] {new object[0], new object[0], new object[0]}
                        };
            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginGetSystemStateStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MMasterProxy.AllInstances.EndGetSystemStateIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));
            var ex = AssertEx.Throws<InvalidOperationException>(() => client.GetSystemStateAsync(null).First());
            ex.Message.Is("caller_id must be a string");
        }


        [TestMethod]
        [HostType("Moles")]
        public void GetUri_Success()
        {
            var result = new object[3]
            {
                1,
                "",
                "http://192.168.11.4:11311/"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginGetUriStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MMasterProxy.AllInstances.EndGetUriIAsyncResult= (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            client.GetUriAsync("/test").First().Is(new Uri("http://192.168.11.4:11311/"));

        }
        [TestMethod]
        [HostType("Moles")]
        public void GetUri_ParameterError()
        {
            var result = new object[3]
            {
                -1,
                "caller_id must be a string",
                ""
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginGetUriStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MMasterProxy.AllInstances.EndGetUriIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<InvalidOperationException>(() => client.GetUriAsync(null).First());
            ex.Message.Is("caller_id must be a string");

        }
    }
}
