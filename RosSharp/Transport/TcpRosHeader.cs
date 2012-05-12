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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
#if !WINDOWS_PHONE
using System.Dynamic;
using Common.Logging;
using RosSharp.Topic;

#endif

namespace RosSharp.Transport
{
    internal sealed class TcpRosHeader : DynamicObject
    {
        private readonly Dictionary<string, string> _members;

        public TcpRosHeader(Dictionary<string, string> members)
        {
            _members = members;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = string.Empty;
            if (!_members.ContainsKey(binder.Name))
            {
                return false;
            }

            result = _members[binder.Name];
            return true;
        }

        public bool HasMember(string name)
        {
            return _members.ContainsKey(name);
        }
    }

    internal static class TcpRosHeaderSerializer
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public static void Serialize(Stream stream, object data)
        {
            var list = data.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => p.Name + "=" + p.GetValue(data, null))
                .ToList();

            if (list.Count == 0)
            {
                _logger.Error("Header does not have properties");
                throw new RosTopicException("Header does not have properties");
            }

            var length = list.Sum(x => x.Length + 4);

            var lenBuf = BitConverter.GetBytes(length);
            stream.Write(lenBuf, 0, lenBuf.Length);

            list.ForEach(x =>
            {
                stream.Write(BitConverter.GetBytes(x.Length), 0, 4);
                var buf = Encoding.UTF8.GetBytes(x);
                stream.Write(buf, 0, buf.Length);
            });
        }

        public static TcpRosHeader Deserialize(Stream stream)
        {
            if (stream.Length < 4)
            {
                _logger.Error("Stream length is too short");
                throw new RosTopicException("Stream length is too short");
            }

            var buf = new byte[4];
            stream.Read(buf, 0, 4);

            var length = BitConverter.ToInt32(buf, 0);

            if (length + 4 != stream.Length)
            {
                _logger.Error(m => m("Stream length mismatch, expected={0} actual={1}", length, stream.Length));
                throw new RosTopicException("Stream length mismatch");
            }

            var map = new Dictionary<string, string>();

            while (stream.Position < length + 4)
            {
                var lenBuf = new byte[4];
                stream.Read(lenBuf, 0, 4);
                var len = BitConverter.ToInt32(lenBuf, 0);

                if (stream.Position + len > length + 4)
                {
                    _logger.Error(m => m("Stream length mismatch, expected={0} actual={1}", length, stream.Length));
                    throw new RosTopicException("Stream length mismatch");
                }

                var dataBuf = new byte[len];
                stream.Read(dataBuf, 0, len);

                var data = Encoding.UTF8.GetString(dataBuf, 0, dataBuf.Length);
                
                var index = data.IndexOf('=');

                if (index < 0)
                {
                    _logger.Error("Header does not contain '='");
                    throw new RosTopicException("Header does not contain '='");
                }

                var first = data.Substring(0, index);
                var last = data.Substring(index + 1, data.Length - (index + 1));

                map.Add(first, last);
            }

            return new TcpRosHeader(map);
        }
    }
}