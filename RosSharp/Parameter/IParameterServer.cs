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

using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    /// <summary>
    ///   Defines interface for ParameterServer API
    /// </summary>
    /// <remarks>
    ///   http://www.ros.org/wiki/ROS/Parameter%20Server%20API
    /// </remarks>
    [XmlRpcUrl("")]
    internal interface IParameterServer
    {
        /// <summary>
        ///   Delete parameter
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        [XmlRpcMethod("deleteParam")]
        object[] DeleteParam(string callerId, string key);

        /// <summary>
        ///   Set parameter.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name. </param>
        /// <param name="value"> Parameter value. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        [XmlRpcMethod("setParam")]
        object[] SetParam(string callerId, string key, object value);

        /// <summary>
        ///   Retrieve parameter value from server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name. If key is a namespace, getParam() will return a parameter tree. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = parameterValue
        /// </returns>
        [XmlRpcMethod("getParam")]
        object[] GetParam(string callerId, string key);

        /// <summary>
        ///   Search for parameter key on the Parameter Server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name to search for. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str: foundKey
        /// </returns>
        [XmlRpcMethod("searchParam")]
        object[] SearchParam(string callerId, string key);

        /// <summary>
        ///   Retrieve parameter value from server and subscribe to updates to that param. See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="callerApi"> Node API URI of subscriber for paramUpdate callbacks. </param>
        /// <param name="key"> Parameter name </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = parameterValue
        /// </returns>
        [XmlRpcMethod("subscribeParam")]
        object[] SubscribeParam(string callerId, string callerApi, string key);

        /// <summary>
        ///   Retrieve parameter value from server and subscribe to updates to that param. See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="callerApi"> Node API URI of subscriber. </param>
        /// <param name="key"> Parameter name. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: number of unsubscribed
        /// </returns>
        [XmlRpcMethod("unsubscribeParam")]
        object[] UnsubscribeParam(string callerId, string callerApi, string key);

        /// <summary>
        ///   Check if parameter is stored on server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="key"> Parameter name. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = bool: hasParam
        /// </returns>
        [XmlRpcMethod("hasParam")]
        object[] HasParam(string callerId, string key);

        /// <summary>
        ///   Get list of all parameter names stored on this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str[]: parameter name list
        /// </returns>
        [XmlRpcMethod("getParamNames")]
        object[] GetParamNames(string callerId);
    }
}