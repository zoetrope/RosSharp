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
using System.Threading.Tasks;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.Topic;
using RosSharp.Parameter;

namespace RosSharp.Node
{
    /// <summary>
    ///   Defines interface for ROS NODE
    /// </summary>
    public interface INode : IDisposable
    {
        /// <summary>
        /// Node ID
        /// </summary>
        string NodeId { get; }

        /// <summary>
        /// Asynchronous Dispose
        /// </summary>
        /// <returns>task object for asynchronous operation</returns>
        Task DisposeAsync();

        /// <summary>
        /// Create a ROS Topic Subscriber
        /// </summary>
        /// <typeparam name="TMessage">Topic Message Type</typeparam>
        /// <param name="topicName">Topic Name</param>
        /// <param name="nodelay">false: Socket uses the Nagle algorithm</param>
        /// <returns>Subscriber</returns>
        Task<Subscriber<TMessage>> CreateSubscriberAsync<TMessage>(string topicName, bool nodelay = true)
            where TMessage : IMessage, new();

        /// <summary>
        /// Create a ROS Topic Publisher
        /// </summary>
        /// <typeparam name="TMessage">Topic Message Type</typeparam>
        /// <param name="topicName">Topic Name</param>
        /// <param name="latching">true: send the latest published message when subscribed topic</param>
        /// <returns>Publisher</returns>
        Task<Publisher<TMessage>> CreatePublisherAsync<TMessage>(string topicName, bool latching = false)
            where TMessage : IMessage, new();

        /// <summary>
        /// Create a Proxy Object for ROS Service 
        /// </summary>
        /// <typeparam name="TService">Service Type</typeparam>
        /// <param name="serviceName">Service Name</param>
        /// <returns>Proxy Object</returns>
        Task<TService> CreateProxyAsync<TService>(string serviceName)
            where TService : IService, new();

        /// <summary>
        /// Register a ROS Service
        /// </summary>
        /// <typeparam name="TService">Service Type</typeparam>
        /// <param name="serviceName">Service Name</param>
        /// <param name="service">Service Instance</param>
        /// <returns>object that dispose a service</returns>
        Task<IServiceServer> RegisterServiceAsync<TService>(string serviceName, TService service)
            where TService : IService, new();
        
        /// <summary>
        /// Create a ROS Parameter
        /// </summary>
        /// <typeparam name="TParameter">Parameter Type</typeparam>
        /// <param name="paramName">Parameter Name</param>
        /// <returns>Parameter</returns>
        Task<Parameter<TParameter>> CreateParameterAsync<TParameter>(string paramName);
    }
}