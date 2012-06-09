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

namespace RosSharp.Service
{
    /// <summary>
    ///   Defines interface for ROS Service
    /// </summary>
    public interface IService : IAsyncDisposable
    {
        /// <summary>
        /// Service Type Name
        /// </summary>
        string ServiceType { get; }

        /// <summary>
        /// MD5 Sum of this Service
        /// </summary>
        string Md5Sum { get; }

        /// <summary>
        /// Raw Service Definition
        /// </summary>
        string ServiceDefinition { get; }

        /// <summary>
        /// Create default Request instance
        /// </summary>
        /// <returns></returns>
        IMessage CreateRequest();

        /// <summary>
        /// Create default Response instance
        /// </summary>
        /// <returns></returns>
        IMessage CreateResponse();

        /// <summary>
        /// Invoke Service
        /// </summary>
        /// <param name="req">Request</param>
        /// <returns>Response</returns>
        IMessage Invoke(IMessage req);

        /// <summary>
        /// Set Service Action
        /// </summary>
        /// <param name="action">Service Action</param>
        void SetAction(Func<IMessage, IMessage> action);
    }

    /// <summary>
    ///   Defines strong typed interface for ROS Service
    /// </summary>
    public interface ITypedService<in TRequest, TResponse> : IService
        where TRequest : IMessage, new()
        where TResponse : IMessage, new()
    {
        /// <summary>
        /// Invoke Service
        /// </summary>
        /// <param name="req">Request</param>
        /// <returns>Response</returns>
        TResponse Invoke(TRequest req);

        /// <summary>
        /// Asynchronous invoke Service
        /// </summary>
        /// <param name="req">Request</param>
        /// <returns>Response</returns>
        Task<TResponse> InvokeAsync(TRequest req);
    }
}