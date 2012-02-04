using System;
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
        public void TestLookupNode_Success()
        {
            var result = new object[3]
            {
                1,
                "node api",
                "http://192.168.11.4:59511/"
            };

            var master = new SIMaster();
            master.LookupNodeStringString = (_, __) => result;
            master.UrlSetString = _ => { };

            MXmlRpcProxyGen.Create<IMaster>(() => master);
            var client = new MasterClient();

            client.LookupNode("/test", "/rosout").Is(new Uri("http://192.168.11.4:59511/"));

        }

        [TestMethod]
        [HostType("Moles")]
        public void TestLookupNode_UnknownError()
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

            AssertEx.Throws<InvalidOperationException>(() => client.LookupNode("/test", "/hogehoge"));
        }
        [TestMethod]
        [HostType("Moles")]
        public void TestLookupNode_ParameterError()
        {
            var result = new object[3]
            {
                -1,
                "ERROR: parameter [node] must be a non-empty string",
                ""
            };

            var master = new SIMaster();
            master.LookupNodeStringString = (_, __) => result;
            master.UrlSetString = _ => { };

            MXmlRpcProxyGen.Create<IMaster>(() => master);
            var client = new MasterClient();

            AssertEx.Throws<InvalidOperationException>(() => client.LookupNode("/test", null));
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestGetSystemState_Success()
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

            var master = new SIMaster();
            master.GetSystemStateString = _ => result;
            master.UrlSetString = _ => { };

            MXmlRpcProxyGen.Create<IMaster>(() => master);

            var client = new MasterClient();

            var state = client.GetSystemState("/test");
            state.Publishers.Count.Is(3);
            state.Subscribers.Count.Is(2);
            state.Services.Count.Is(2);
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestGetSystemState_ParameterError()
        {
            var result = new object[3]
                        {
                            -1,
                            "caller_id must be a string",
                            new object[3] {new object[0], new object[0], new object[0]}
                        };
            var master = new SIMaster();
            master.GetSystemStateString = _ => result;
            master.UrlSetString = _ => { };

            MXmlRpcProxyGen.Create<IMaster>(() => master);
            var client = new MasterClient();

            AssertEx.Throws<InvalidOperationException>(() => client.GetSystemState(null));
        }
    }
}
