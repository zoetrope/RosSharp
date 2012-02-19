using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using CookComputing.XmlRpc;

namespace RosSharp
{
    public class RosNode : INode
    {
        private MasterClient _masterClient;
        private SlaveServer _slaveServer;
        public RosNode()
        {
            _masterClient = new MasterClient(new Uri("http://192.168.11.5:11311/"));


        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) where TDataType : IMessage, new()
        {
            var ret1 = _masterClient
                .RegisterSubscriberAsync("/test", "chatter", "std_msgs/String", new Uri("http://192.168.11.2:11112"))
                .First();//TODO: エラーが起きたとき

            var slave = new SlaveClient(ret1.First());

            var topicParam = slave.RequestTopicAsync("/test", "/chatter", new object[1] { new string[1] { "TCPROS" } }).First();

            var subscriber = new Subscriber<TDataType>(topicParam);

            return subscriber;
        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(GraphName topicName) where TDataType : IMessage, new()
        {
            throw new NotImplementedException();
        }

        public Publisher<TDataType> CreatePublisher<TDataType>(string topicName) where TDataType : IMessage, new()
        {
            _slaveServer = new SlaveServer();

            var publisher = new Publisher<TDataType>();

            _slaveServer.Connect().Subscribe(x => publisher.AddTopic(new RosTopic<TDataType>(x)));

            var channel = new HttpServerChannel("slave", 5678, new XmlRpcServerFormatterSinkProvider());
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(_slaveServer, "slave");


            var ret1 = _masterClient.RegisterPublisherAsync("/test", "/chatter", "std_msgs/String", new Uri("http://192.168.11.4:5678/slave")).First();

            

            return publisher;
        }

        public Publisher<TDataType> CreatePublisher<TDataType>(GraphName topicName) where TDataType : IMessage, new()
        {
            throw new NotImplementedException();
        }
    }
}
