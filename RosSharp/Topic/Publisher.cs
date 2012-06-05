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
    public sealed class Publisher<TMessage> : IPublisher, IObserver<TMessage>
        where TMessage : IMessage, new()
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly BehaviorSubject<int> _connectionCounterSubject = new BehaviorSubject<int>(0);
        private readonly List<RosTopicClient<TMessage>> _rosTopicClients = new List<RosTopicClient<TMessage>>();
        private readonly bool _latching;
        private TMessage _lastPublishedMessage;

        internal Publisher(string topicName, string nodeId, bool latching = false)
        {
            var dummy = new TMessage();
            TopicName = topicName;
            MessageType = dummy.MessageType;
            NodeId = nodeId;
            _latching = latching;
        }

        public string NodeId { get; private set; }

        public Task DisposeAsync()
        {
            lock (_rosTopicClients)
            {
                foreach (var topic in _rosTopicClients)
                {
                    topic.Dispose();
                }
                _rosTopicClients.Clear();
            }

            var handler = Disposing;
            Disposing = null;
            return handler(TopicName);
        }

        #region IDisposable Members

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        #endregion

        #region IObserver<TMessage> Members

        public void OnNext(TMessage value)
        {
            lock (_rosTopicClients) //ロック範囲が広い？
            {
                foreach (var client in _rosTopicClients)
                {
                    try
                    {
                        client.SendAsync(value).Wait();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("SendError", ex);
                    }
                }
                _lastPublishedMessage = value;
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

        public IObservable<int> ConnectionCounterChangedAsObservable()
        {
            return _connectionCounterSubject;
        }

        internal Task AddTopic(Socket socket)
        {
            var rosTopicClient = new RosTopicClient<TMessage>(NodeId, TopicName);

            return rosTopicClient.StartAsync(socket, _latching)
                .ContinueWith(startTask =>
                {
                    if(startTask.Status == TaskStatus.RanToCompletion)
                    {
                        _logger.Debug("AddTopic: Started");

                        startTask.Result.Subscribe(
                            _ => { },
                            ex=>
                            {
                                lock (_rosTopicClients)
                                {
                                    _rosTopicClients.Remove(rosTopicClient);
                                    _connectionCounterSubject.OnNext(_rosTopicClients.Count);
                                }
                            }
                            );

                        lock (_rosTopicClients)
                        {
                            _rosTopicClients.Add(rosTopicClient);
                        }

                        if(_latching && _lastPublishedMessage != null)
                        {
                            OnNext(_lastPublishedMessage);
                        }

                        _logger.Debug("OnConnected");
                        _connectionCounterSubject.OnNext(_rosTopicClients.Count);
                    }
                    else if (startTask.Status == TaskStatus.Faulted)
                    {
                        _logger.Error("AddTopic: Failure", startTask.Exception.InnerException);
                        throw startTask.Exception.InnerException;
                    }
                });
        }

        internal void UpdateSubscribers(List<Uri> subscribers)
        {
            //TODO: 不要では？
        }

        internal event Func<string, Task> Disposing = s => Task.Factory.StartNew(() => { });
    }
}