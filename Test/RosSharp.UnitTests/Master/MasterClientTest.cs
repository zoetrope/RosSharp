using System;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;

namespace RosSharp.Tests.Master
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
                StatusCode.Success,
                "Registered [/test] as provider of [/myservice]",
                1
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginRegisterServiceStringStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6, t7) => { t6(null); return null; };
            MMasterProxy.AllInstances.EndRegisterServiceIAsyncResult= (t1, t2) => result;
            
            var client = new MasterClient(new Uri("http://localhost"));

            client.RegisterServiceAsync("/test", "/myservice", new Uri("http://192.168.11.2:11112"), new Uri("http://192.168.11.2:11111")).Wait();
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

            var ex = AssertEx.Throws<AggregateException>(() => client.RegisterServiceAsync("/test", "/myservice", new Uri("http://localhost"), new Uri("http://localhost")).Wait());
            ex.InnerException.Message.Is("ERROR: parameter [service_api] is not an XMLRPC URI");
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

            client.UnregisterServiceAsync("/test", "/myservice", new Uri("http://localhost")).Result.Is(1);
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

            client.UnregisterServiceAsync("/test", "/myservice", new Uri("http://localhost")).Result.Is(0);
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

            var ex = AssertEx.Throws<AggregateException>(() => client.UnregisterServiceAsync("/test", "/myservice", new Uri("http://localhost")).Wait());
            ex.InnerException.Message.Is("ERROR: parameter [service] must be a non-empty string");
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

            client.RegisterSubscriberAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11112")).Wait();
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

            var ex = AssertEx.Throws<AggregateException>(() => client.RegisterSubscriberAsync("/test", "topic1", "topicType", new Uri("http://192.168.11.2:11112")).Wait());
            ex.InnerException.Message.Is("ERROR: parameter [topic_type] is not a valid package resource name");
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

            client.RegisterPublisherAsync("/test", "topic1", "std_msgs/String", new Uri("http://192.168.11.2:11113"))
                .Result[0].Is(new Uri("http://192.168.11.2:11112/"));
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

            client.LookupNodeAsync("/test", "/rosout").Result.Is(new Uri("http://192.168.11.4:59511/"));
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

            var ex = AssertEx.Throws<AggregateException>(() => client.LookupNodeAsync("/test", "/hogehoge").Wait());
            ex.InnerException.Message.Is("unknown node [/hogehoge]");
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

            var ex = AssertEx.Throws<AggregateException>(() => client.LookupNodeAsync("/test", null).Wait());
            ex.InnerException.Message.Is("ERROR: parameter [node] must be a non-empty string");
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

            var ret = client.GetSystemStateAsync("/test").Result;
            ret.Publishers.Count.Is(3);
            ret.Subscribers.Count.Is(2);
            ret.Services.Count.Is(2);
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
            var ex = AssertEx.Throws<AggregateException>(() => client.GetSystemStateAsync(null).Wait());
            ex.InnerException.Message.Is("caller_id must be a string");
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

            client.GetUriAsync("/test").Result.Is(new Uri("http://192.168.11.4:11311/"));
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

            var ex = AssertEx.Throws<AggregateException>(() => client.GetUriAsync(null).Wait());
            ex.InnerException.Message.Is("caller_id must be a string");

        }


        [TestMethod]
        [HostType("Moles")]
        public void LookupService_Success()
        {
            var result = new object[3]
            {
                1,
                "rosrpc URI: [rosrpc://192.168.11.5:37171]",
                "rosrpc://192.168.11.5:37171"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginLookupServiceStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MMasterProxy.AllInstances.EndLookupServiceIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            client.LookupServiceAsync("/test", "/service_test").Result.Is(new Uri("rosrpc://192.168.11.5:37171"));
        }

        [TestMethod]
        [HostType("Moles")]
        public void LookupService_NoProvider()
        {
            var result = new object[3]
            {
                -1,
                "no provider",
                ""
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MMasterProxy.AllInstances.BeginLookupServiceStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MMasterProxy.AllInstances.EndLookupServiceIAsyncResult = (t1, t2) => result;

            var client = new MasterClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<AggregateException>(() => client.LookupServiceAsync("/test", "/service_test").Wait());
            ex.InnerException.Message.Is("no provider");
        }
    }
}
