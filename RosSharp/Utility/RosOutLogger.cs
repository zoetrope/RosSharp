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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;
using RosSharp.Node;
using RosSharp.Topic;
using RosSharp.Utility;
using RosSharp.rosgraph_msgs;

namespace RosSharp
{
    /// <summary>
    /// Logging to RosOut
    /// </summary>
    internal sealed class RosOutLogger : AbstractSimpleLogger
    {
        private readonly RosNode _node;
        private readonly Publisher<Log> _publisher;
        
        internal RosOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName,
                            string dateTimeFormat)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {

            _node = Ros.GetNodes().OfType<RosNode>().FirstOrDefault(x => x.NodeId == logName);

            if (_node != null)
            {
                _publisher = _node.LogPubliser;
            }

            
        }

        protected override void WriteInternal(LogLevel targetLevel, object message, Exception e)
        {
            var sb = new StringBuilder();
            FormatOutput(sb, targetLevel, message, e);
            LogLevel currentLevel = LogLevel.Info;

            if (_node != null)
            {
                currentLevel = _node.LogLevel.ToLogLevel();
                //_node.
            }

            if (_publisher != null)
            {
                if (targetLevel >= currentLevel)
                {
                    _publisher.OnNext(new Log() {msg = message.ToString()});
                }
            }

            //Console.WriteLine(sb.ToString());
        }
    }
}