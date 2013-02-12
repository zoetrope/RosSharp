using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Parameter;

namespace RosSharp.Tests.Parameter
{
    [TestClass]
    public class ParameterServerClientTest
    {

        [TestMethod]
        [HostType("Moles")]
        public void DeleteParam_Success()
        {
            var result = new object[3]
            {
                1,
                "parameter /test_param deleted",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginDeleteParamStringStringAsyncCallbackObject= (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndDeleteParamIAsyncResult= (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.DeleteParamAsync("test", "test_param").Wait();
        }

        [TestMethod]
        [HostType("Moles")]
        public void DeleteParam_NotSet()
        {
            var result = new object[3]
            {
                -1,
                "parameter [/aaa] is not set",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginDeleteParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndDeleteParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));

            var ex = AssertEx.Throws<AggregateException>(
                () => client.DeleteParamAsync("test", "aaa").Wait());
            ex.InnerException.Message.Is("parameter [/aaa] is not set");
        }


        [TestMethod]
        [HostType("Moles")]
        public void SetParam_Success()
        {
            var result = new object[3]
            {
                1,
                "Parameter [/rosversion]",
                "1.6.5"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginSetParamStringStringObjectAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MParameterServerProxy.AllInstances.EndSetParamIAsyncResult= (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.SetParamAsync("test", "/test_param", 1234).Wait();
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetParam_SuccessString()
        {
            var result = new object[3]
            {
                1,
                "Parameter [/rosversion]",
                "1.6.5"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginGetParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndGetParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.GetParamAsync("test", "rosversion").Result.Is("1.6.5");
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetParam_SuccessList()
        {
            // > rosparam set /foo "['1', 1, 1.0]"

            var result = new object[3]
            {
                1,
                "Parameter [/foo]",
                new object[3]
                {
                    "1",
                    1,
                    1.0
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginGetParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndGetParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            var ret = client.GetParamAsync("test", "foo").Result as object[];
            ret[0].Is("1");
            ret[1].Is(1);
            ret[2].Is(1.0);
        }
        
        [TestMethod]
        [HostType("Moles")]
        public void GetParam_SuccessDictionary()
        {
            // rosparam set /gains "p: 1.0
            // i: 1.0
            // d: 1.0"

            var result = new object[3]
            {
                1,
                "Parameter [/gains]",
                new object[3]
                {
                    "1",
                    1,
                    1.0
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginGetParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndGetParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.GetParamAsync("test", "gains").Wait();
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetParam_NotSet()
        {
            var result = new object[3]
            {
                -1,
                "Parameter [/aaa] is not set",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginGetParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndGetParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            
            var ex = AssertEx.Throws<AggregateException>(
                () => client.GetParamAsync("test", "aaa").Wait());
            ex.InnerException.Message.Is("Parameter [/aaa] is not set");
        }


        [TestMethod]
        [HostType("Moles")]
        public void SearchParam_Failure()
        {
            var result = new object[3]
            {
                0,
                "Internal failure: namespace must be global",
                0
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginSearchParamStringStringAsyncCallbackObject= (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndSearchParamIAsyncResult= (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            
            var ex = AssertEx.Throws<AggregateException>(
                () => client.SearchParamAsync("test", "rosversion").Wait());
            ex.InnerException.Message.Is("Internal failure: namespace must be global");
        }

        [TestMethod]
        [HostType("Moles")]
        public void SubscribeParam_Success()
        {
            var result = new object[3]
            {
                1,
                "Subscribed to parameter [/rosversion]",
                "1.6.5"
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginSubscribeParamStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MParameterServerProxy.AllInstances.EndSubscribeParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.SubscribeParamAsync("test", new Uri("http://localhost:11311"), "rosversion").Wait();
        }
        
        //var version = client.SubscribeParamAsync("test", server.SlaveUri, "aaa").First();
        //[0] = 1
        //[1] = "Subscribed to parameter [/aaa]"
        //[2] = {CookComputing.XmlRpc.XmlRpcStruct}

        [TestMethod]
        [HostType("Moles")]
        public void UnsubscribeParam_Success()
        {
            var result = new object[3]
            {
                1,
                "Unsubscribe to parameter [/rosversion]",
                1
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginUnsubscribeParamStringStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5, t6) => { t5(null); return null; };
            MParameterServerProxy.AllInstances.EndUnsubscribeParamIAsyncResult= (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.UnsubscribeParamAsync("test", new Uri("http://localhost:11311"), "rosversion").Result.Is(1);
        }
        
        //var version = client.UnsubscribeParamAsync("test", server.SlaveUri, "bbb").First();
        //[0] = 1
        //[1] = "Unsubscribe to parameter [/bbb]"
        //[2] = 1

        //var server = new SlaveServer(0, new TopicContainer(), new RosTopicServer());
        //var a = client.SetParamAsync("test", "test_param", 123).First();
        //var b = client.SubscribeParamAsync("test", server.SlaveUri, "test_param").First();
        //var c = client.SetParamAsync("test", "test_param", 456).First();
        //Slave.ParamUpdateが呼ばれる。


        [TestMethod]
        [HostType("Moles")]
        public void HasParam_SuccessTrue()
        {
            var result = new object[3]
            {
                1,
                "/rosversion",
                true
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginHasParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndHasParamIAsyncResult= (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.HasParamAsync("test", "/rosversion").Result.Is(true);
        }

        [TestMethod]
        [HostType("Moles")]
        public void HasParam_SuccessFalse()
        {
            var result = new object[3]
            {
                1,
                "/aaa",
                false
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginHasParamStringStringAsyncCallbackObject = (t1, t2, t3, t4, t5) => { t4(null); return null; };
            MParameterServerProxy.AllInstances.EndHasParamIAsyncResult = (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.HasParamAsync("test", "/aaa").Result.Is(false);
        }

        [TestMethod]
        [HostType("Moles")]
        public void GetParamNames_Success()
        {
            var result = new object[3]
            {
                1,
                "Parameter names",
                new string[4]
                {
                    "/roslaunch/uris/host_192_168_11_6__51664",
                    "/rosversion",
                    "/rosdistro",
                    "/run_id"
                }
            };

            MXmlRpcClientProtocol.AllInstances.UrlSetString = (t1, t2) => { };
            MParameterServerProxy.AllInstances.BeginGetParamNamesStringAsyncCallbackObject = (t1, t2, t3, t4) => { t3(null); return null; };
            MParameterServerProxy.AllInstances.EndGetParamNamesIAsyncResult= (t1, t2) => result;

            var client = new ParameterServerClient(new Uri("http://localhost"));
            client.GetParamNamesAsync("test").Result.Is(
                new string[] {"/roslaunch/uris/host_192_168_11_6__51664","/rosversion","/rosdistro","/run_id"});

        }



    }
}

