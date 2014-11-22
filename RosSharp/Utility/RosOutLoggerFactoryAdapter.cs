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
using System.Text;
using Common.Logging;
using Common.Logging.Simple;
using Common.Logging.Configuration;

namespace RosSharp
{
    /// <summary>
    /// LoggerFactoryAdapter for use of the RosOutLogger
    /// </summary>
    public sealed class RosOutLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        public RosOutLoggerFactoryAdapter()
            : base(null)
        {
            
        }

        public RosOutLoggerFactoryAdapter(NameValueCollection properties)
            : base(properties)
        {
        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime,
                                             bool showLogName, string dateTimeFormat)
        {
            var names = name.Split('@');

            string nodeId;
            string typeName;

            if(names.Length == 2)
            {
                typeName = names[0];
                nodeId = names[1];
            }
            else
            {
                typeName = name;
                nodeId = "unknown";
            }

            return new RosOutLogger(typeName, nodeId, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
    }
}