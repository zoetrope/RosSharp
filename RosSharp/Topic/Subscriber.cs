using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RosSharp.Message;
using RosSharp.Slave;

namespace RosSharp.Topic
{
    /// <summary>
    ///   Subscribes message on a ROS Topic
    /// </summary>
    /// <typeparam name="TDataType"> Message Type </typeparam>
    public sealed class Subscriber<TDataType> : ISubscriber, IObservable<TDataType>, IDisposable
        where TDataType : IMessage, new()
    {
        private Subject<TDataType> _aggregateSubject = new Subject<TDataType>();
        private CompositeDisposable _disposables = new CompositeDisposable();

        internal Subscriber(string topicName, string nodeId)
        {
            TopicName = topicName;
            var dummy = new TDataType();
            MessageType = dummy.MessageType;

            NodeId = nodeId;
        }

        public string NodeId { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            var handler = Disposing;
            if (handler != null)
            {
                handler();
            }

            lock (_disposables)
            {
                _disposables.Dispose();
            }
        }

        #endregion

        #region IObservable<TDataType> Members

        public IDisposable Subscribe(IObserver<TDataType> observer)
        {
            return _aggregateSubject.Subscribe(observer);
        }

        #endregion

        #region ISubscriber Members

        void ISubscriber.UpdatePublishers(List<Uri> publishers)
        {
            //TODO: 同じPublisherに対する処理
            var slaves = publishers.Select(x => new SlaveClient(x));

            Parallel.ForEach(slaves,
                             slave => slave.RequestTopicAsync(NodeId, TopicName, new object[1] {new string[1] {"TCPROS"}})
                                          .ContinueWith(task => Connect(task.Result), TaskContinuationOptions.OnlyOnRanToCompletion));
        }

        public string TopicName { get; private set; }

        public string MessageType { get; private set; }

        #endregion

        private void Connect(TopicParam param)
        {
            //TODO: serverを複数持てるようにする。保持しておく。ロックも必要。
            var server = new RosTopicServer<TDataType>(TopicName, NodeId);

            server.StartAsync(param).ContinueWith(
                task =>
                {
                    var d = task.Result.Subscribe(_aggregateSubject);
                    lock (_disposables)
                    {
                        _disposables.Add(d);
                    }
                    var handler = OnConnected;
                    if (handler != null)
                    {
                        handler();
                    }
                });
        }

        public event Action OnConnected;

        internal event Action Disposing;
    }
}