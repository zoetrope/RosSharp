using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CookComputing.XmlRpc.Moles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Moles;

namespace RosSharp.Tests
{
    [TestClass]
    public class MasterClientTest
    {
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

            var client = new MasterClient();

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

            var master = new SIMaster();
            master.LookupNodeStringString = (_, __) => result;
            master.UrlSetString = _ => { };

            MXmlRpcProxyGen.Create<IMaster>(() => master);
            var client = new MasterClient();

            AssertEx.Throws<InvalidOperationException>(() => client.LookupNodeAsync("/test", "/hogehoge").First());
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

            var client = new MasterClient();

            AssertEx.Throws<InvalidOperationException>(() => client.LookupNodeAsync("/test", null).First());
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

            var client = new MasterClient();

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

            var client = new MasterClient();
            AssertEx.Throws<InvalidOperationException>(() => client.GetSystemStateAsync(null).First());
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

            var client = new MasterClient();

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

            var client = new MasterClient();

            AssertEx.Throws<InvalidOperationException>(() => client.GetUriAsync(null).First());

        }
    }
}
