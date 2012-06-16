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
using System.Reactive.Linq;
using System.Text;
using Common.Logging;
using RosSharp.roscpp;
using RosSharp.rosgraph_msgs;

namespace RosSharp
{
    /// <summary>
    /// Logging ROS Node
    /// </summary>
    public sealed class RosOut
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Start()
        {
            var node = Ros.InitNodeAsync("/rosout", enableLogger:false).Result;

            var publisher = node.PublisherAsync<Log>("/rosout_agg").Result;
            var subscriber = node.SubscriberAsync<Log>("/rosout").Result;


            subscriber.Subscribe(x =>
            {
                Console.WriteLine(x);
            });
            
            var d = subscriber.Publish(xs =>
            {
                xs.Where(x => x.level == Log.DEBUG).Subscribe(x => _logger.Debug(m => m("Node = {0}, Message = {1}", x.name, x.msg)));
                xs.Where(x => x.level == Log.INFO).Subscribe(x => _logger.Info(m => m("Node = {0}, Message = {1}", x.name, x.msg)));
                xs.Where(x => x.level == Log.WARN).Subscribe(x => _logger.Warn(m => m("Node = {0}, Message = {1}", x.name, x.msg)));
                xs.Where(x => x.level == Log.ERROR).Subscribe(x => _logger.Error(m => m("Node = {0}, Message = {1}", x.name, x.msg)));
                xs.Where(x => x.level == Log.FATAL).Subscribe(x => _logger.Fatal(m => m("Node = {0}, Message = {1}", x.name, x.msg)));
                return xs;
            }).Subscribe();

        }
    }
}