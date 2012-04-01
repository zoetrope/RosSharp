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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Message;

namespace RosSharp.Topic
{
    internal sealed class TopicContainer
    {
        //TODO: concurrentではなく、呼び出す側でロックすべきか。
        private readonly ConcurrentDictionary<string, IPublisher> _publishers = new ConcurrentDictionary<string, IPublisher>();
        private readonly ConcurrentDictionary<string, ISubscriber> _subscribers = new ConcurrentDictionary<string, ISubscriber>();

        public bool AddPublisher(IPublisher publisher)
        {
            return _publishers.TryAdd(publisher.TopicName, publisher);
        }

        public bool RemovePublisher(string topicName)
        {
            IPublisher dummy;
            return _publishers.TryRemove(topicName, out dummy);
        }

        public bool AddSubscriber(ISubscriber subscriber)
        {
            return _subscribers.TryAdd(subscriber.TopicName, subscriber);
        }

        public bool RemoveSubscriber(string topicName)
        {
            ISubscriber dummy;
            return _subscribers.TryRemove(topicName, out dummy);
        }

        public List<IPublisher> GetPublishers()
        {
            return _publishers.Values.ToList();
        }

        public bool GetPublisher(string topicName, out IPublisher publisher)
        {
            return _publishers.TryGetValue(topicName, out publisher);
        }

        public List<ISubscriber> GetSubscribers()
        {
            return _subscribers.Values.ToList();
        }

        public bool GetSubscriber(string topicName, out ISubscriber subscriber)
        {
            return _subscribers.TryGetValue(topicName, out subscriber);
        }

        public bool HasPublisher(string topic)
        {
            return _publishers.ContainsKey(topic);
        }

        public bool HasSubscriber(string topic)
        {
            return _subscribers.ContainsKey(topic);
        }
    }
}