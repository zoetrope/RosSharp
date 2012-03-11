using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using CookComputing.XmlRpc;
using RosSharp.Master;

namespace RosSharp.Master
{
    public class MasterServer : MarshalByRefObject, IMaster
    {
        private RegistrationContainer _registrationContainer;


        public Uri MasterUri { get; private set; }

        public MasterServer(int portNumber)
        {
            _registrationContainer = new RegistrationContainer();

            //var channel = new HttpServerChannel("master", portNumber, new XmlRpcServerFormatterSinkProvider());
            var channel = new HttpServerChannel("master", portNumber, new XmlRpcServerFormatterSinkProvider());
            
            var tmp = new Uri(channel.GetChannelUri());

            MasterUri = new Uri("http://" + ROS.LocalHostName + ":" + tmp.Port);

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(this, "/");


        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public object[] RegisterService(string callerId, string service, string serviceApi, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterService(string callerId, string service, string serviceApi)
        {
            throw new NotImplementedException();
        }

        public object[] RegisterSubscriber(string callerId, string topic, string topicType, string callerApi)
        {
            var uris = _registrationContainer.RegsiterSubscriber(topic, topicType, callerApi);

            return new object[3]
            {
                1,
                "Subscribed to [" + topic + "]",
                uris.Select(x => x.ToString()).ToArray()
            };
        }

        public object[] UnregisterSubscriber(string callerId, string topic, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] RegisterPublisher(string callerId, string topic, string topicType, string callerApi)
        {
            var uris = _registrationContainer.RegisterPublisher(topic, topicType, callerApi);

            return new object[3]
            {
                1,
                "Registered [" + callerId + "] as publisher of [" + topic + "]",
                uris.Select(x => x.ToString()).ToArray()
            };
        }

        public object[] UnregisterPublisher(string callerId, string topic, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] LookupNode(string callerId, string nodeName)
        {
            throw new NotImplementedException();
        }

        public object[] GetPublisherTopics(string callerId, string subgraph)
        {
            throw new NotImplementedException();
        }

        public object[] GetSystemState(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetMasterUri(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] LookupService(string callerId, string service)
        {
            throw new NotImplementedException();
        }
    }

    public class PublisherInfo
    {
        
    }
}
