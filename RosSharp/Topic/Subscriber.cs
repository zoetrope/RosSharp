#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
    /// <typeparam name="TMessage"> Message Type </typeparam>
    public sealed class Subscriber<TMessage> : ISubscriber, IObservable<TMessage>, IDisposable
        where TMessage : IMessage, new()
    {
        private readonly Subject<TMessage> _aggregateSubject = new Subject<TMessage>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ReplaySubject<Unit> _onConnectedSubject = new ReplaySubject<Unit>();
        private readonly List<RosTopicServer<TMessage>> _rosTopicServers = new List<RosTopicServer<TMessage>>();

        internal Subscriber(string topicName, string nodeId)
        {
            TopicName = topicName;
            var dummy = new TMessage();
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
                handler(this);
            }
            Disposing = null;

            lock (_disposables)
            {
                _disposables.Dispose();
            }
        }

        #endregion

        #region IObservable<TMessage> Members

        public IDisposable Subscribe(IObserver<TMessage> observer)
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
                             slave => slave.RequestTopicAsync(NodeId, TopicName, new List<ProtocolInfo> {new ProtocolInfo(ProtocolType.TCPROS)})
                                          .ContinueWith(task => ConnectServer(task.Result), TaskContinuationOptions.OnlyOnRanToCompletion));
        }

        public string TopicName { get; private set; }

        public string MessageType { get; private set; }

        #endregion

        private void ConnectServer(TopicParam param)
        {
            //TODO: serverを複数持てるようにする。保持しておく。ロックも必要。
            var server = new RosTopicServer<TMessage>(NodeId, TopicName);
            _rosTopicServers.Add(server);

            server.StartAsync(param).ContinueWith(
                task =>
                {
                    var d = task.Result.Subscribe(_aggregateSubject);
                    lock (_disposables)
                    {
                        _disposables.Add(d);
                    }
                    _onConnectedSubject.OnNext(Unit.Default);
                });
        }

        public IObservable<Unit> OnConnectedAsObservable()
        {
            return _onConnectedSubject;
        }

        internal event Action<ISubscriber> Disposing;
    }
}