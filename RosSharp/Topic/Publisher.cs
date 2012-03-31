using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;

namespace RosSharp.Topic
{
    /// <summary>
    /// Publishes message on a ROS Topic
    /// </summary>
    /// <typeparam name="TDataType">Message Type</typeparam>
    public sealed class Publisher<TDataType> : IPublisher, IObserver<TDataType>, IDisposable
        where TDataType : IMessage, new()
    {
        private List<RosTopicClient<TDataType>> _rosTopicClients = new List<RosTopicClient<TDataType>>();
        private ILog _logger = LogManager.GetCurrentClassLogger();

        internal Publisher(string topicName, string nodeId)
        {
            var dummy = new TDataType();

            TopicName = topicName;
            MessageType = dummy.MessageType;

            NodeId = nodeId;
        }

        public string NodeId { get; private set; }

        public string TopicName { get; private set; }

        public string MessageType { get; private set; }

        public void OnNext(TDataType value)
        {
            lock (_rosTopicClients) //ロック範囲が広い？
            {
                Parallel.ForEach(_rosTopicClients, client =>
                {
                    client.SendTaskAsync(value).ContinueWith(task =>
                    {
                        _logger.Error("Send Error", task.Exception.InnerException);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                });
            }
        }

        public void OnError(Exception error)
        {
            _logger.Error("OnError", error);
        }

        public void OnCompleted()
        {
            _logger.Info("OnCompleted");
        }

        public event Action OnConnected;

        internal Task AddTopic(Socket socket)
        {
            var rosTopicClient = new RosTopicClient<TDataType>(TopicName, NodeId);
            return rosTopicClient.StartAsync(socket)
                .ContinueWith(task =>
                {
                    lock (_rosTopicClients)
                    {
                        _rosTopicClients.Add(rosTopicClient);
                    }
                    var handler = OnConnected;
                    if (handler != null)
                    {
                        handler();
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        internal void UpdateSubscribers(List<Uri> subscribers)
        {
            //TODO: 不要では？
        }

        internal event Action Disposing;

        public void Dispose()
        {
            var handler = Disposing;
            if (handler != null)
            {
                handler();
            }
            Disposing = null;

            lock (_rosTopicClients)
            {
                foreach (var topic in _rosTopicClients)
                {
                    topic.Dispose();
                }
                _rosTopicClients.Clear();
            }
        }
    }
}
