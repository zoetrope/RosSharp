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
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;
using System.Linq;

namespace RosSharp.Topic
{
    /// <summary>
    ///   Publishes message on a ROS Topic
    /// </summary>
    /// <typeparam name="TMessage"> Message Type </typeparam>
    public sealed class Publisher<TMessage> : IPublisher, IObserver<TMessage>, IDisposable
        where TMessage : IMessage, new()
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly ReplaySubject<Unit> _onConnectedSubject = new ReplaySubject<Unit>();
        private readonly List<RosTopicClient<TMessage>> _rosTopicClients = new List<RosTopicClient<TMessage>>();

        internal Publisher(string topicName, string nodeId)
        {
            var dummy = new TMessage();

            TopicName = topicName;
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

            lock (_rosTopicClients)
            {
                foreach (var topic in _rosTopicClients)
                {
                    topic.Dispose();
                }
                _rosTopicClients.Clear();
            }
        }

        #endregion

        #region IObserver<TMessage> Members

        public void OnNext(TMessage value)
        {
            lock (_rosTopicClients) //ロック範囲が広い？
            {
                //TODO: 送信でエラーが起きると次のやつに送信しない。SendTaskAsync自体が例外をはいている。
                //Task.WaitAll(_rosTopicClients.Select(client => client.SendTaskAsync(value)).ToArray());

                foreach (var client in _rosTopicClients)
                {
                    try
                    {
                        client.SendTaskAsync(value).Wait();
                    }
                    catch (Exception)
                    {
                        _logger.Error("SendError");
                    }
                }

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

        #endregion

        #region IPublisher Members

        public string TopicName { get; private set; }

        public string MessageType { get; private set; }

        #endregion

        public IObservable<Unit> OnConnectedAsObservable()
        {
            return _onConnectedSubject;
        }

        internal Task AddTopic(Socket socket)
        {
            _logger.Debug(m => m("AddTopic: {0}", socket.RemoteEndPoint.ToString()));
            var rosTopicClient = new RosTopicClient<TMessage>(NodeId, TopicName);
            return rosTopicClient.StartAsync(socket)
                .ContinueWith(task =>
                {
                    _logger.Debug("AddTopic: Started");
                    lock (_rosTopicClients)
                    {
                        _rosTopicClients.Add(rosTopicClient);
                    }
                    _onConnectedSubject.OnNext(Unit.Default);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        internal void UpdateSubscribers(List<Uri> subscribers)
        {
            //TODO: 不要では？
        }

        internal event Action<IPublisher> Disposing;
    }
}