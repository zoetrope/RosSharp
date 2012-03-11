using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Slave
{
    public class SlaveServer : MarshalByRefObject, ISlave
    {
        private readonly TopicContainer _topicContainer;
        private readonly RosTopicServer _rosTopicServer;

        public Uri SlaveUri { get; set; }

        public SlaveServer(int portNumber, TopicContainer topicContainer, RosTopicServer topicServer)
        {
            _topicContainer = topicContainer;
            _rosTopicServer = topicServer;

            var channel = new HttpServerChannel("slave", portNumber, new XmlRpcServerFormatterSinkProvider());
            var tmp = new Uri(channel.GetChannelUri());

            SlaveUri = new Uri("http://" + ROS.LocalHostName + ":" + tmp.Port + "/slave");

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(this, "slave");
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public object[] GetBusStats(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetBusInfo(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetMasterUri(string callerId)
        {
            return new object[3]
            {
                1,
                "",
                ROS.MasterUri //TODO: この実装でよい？
            };
        }

        public object[] Shutdown(string callerId, string msg)
        {
            throw new NotImplementedException();
        }

        public object[] GetPid(string callerId)
        {
            return new object[3]
            {
                1,
                "",
                Process.GetCurrentProcess().Id
            };            
        }

        public object[] GetSubscriptions(string callerId)
        {
            return new object[]
            {
                1,
                "Success",
                _topicContainer.GetSubscribers().Select(x => new object[] {x.Name, x.Type}).ToArray()
            };
        }

        public object[] GetPublications(string callerId)
        {
            return new object[]
            {
                1,
                "Success",
                _topicContainer.GetPublishers().Select(x => new object[] {x.Name, x.Type}).ToArray()
            };
        }

        public object[] ParamUpdate(string callerId, string parameterKey, object parameterValue)
        {
            throw new NotImplementedException();
        }

        public object[] PublisherUpdate(string callerId, string topic, string[] publishers)
        {
            if(_topicContainer.HasSubscriber(topic))
            {
                var subs = _topicContainer.GetSubscribers().First(s => s.Name == topic);

                //TODO: publishersを渡す。
                subs.UpdatePublishers();
            }

            //TODO: 戻り値は？
            return new object[0];
        }

        public object[] RequestTopic(string callerId, string topic, object[] protocols)
        {
            if(!_topicContainer.HasPublisher(topic))
            {
                return new object[]
                {
                    -1,
                    "No publishers for topic: " + topic,
                    "null"
                };
            }

            foreach (string[] protocol in protocols)
            {
                string protocolName = protocol[0];

                if (protocolName != "TCPROS") //TODO: ほかのプロトコルにも対応できるように
                {
                    continue;
                }

                var address = _rosTopicServer.AdvertiseAddress;
                
                return new object[3]
                {
                    1,
                    "Protocol<" + protocolName + ", AdvertiseAddress<" + address.ToString() + ">>",
                    new object[3]
                    {
                        protocolName,
                        ROS.LocalHostName,
                        address.Port
                    }
                };
            }

            return new object[]
            {
                -1,
                "No supported protocols specified.",
                "null"
            };

        }
    }
}
