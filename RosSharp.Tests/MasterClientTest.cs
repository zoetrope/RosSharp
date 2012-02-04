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
        public void TestGetSystemState()
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
            master.GetSystemStateString = x => result;

            MXmlRpcProxyGen.Create<IMaster>(() => master);

            var client = new MasterClient();

            var state = client.GetSystemState("/chatter");
            state.Code.Is(1);
            state.StatusMessage.Is("current system state");
            state.Publishers.Count.Is(3);
            state.Subscribers.Count.Is(3);
            state.Services.Count.Is(2);
        }
    }
}
