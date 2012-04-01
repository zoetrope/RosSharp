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
using System.Text;

namespace RosSharp.Message
{
    public static class SerializeExtensions
    {
        private const long UnixEpochBase = 621355968000000000;

        public static void WriteUtf8String(this BinaryWriter bw, string data)
        {
            var buf = Encoding.UTF8.GetBytes(data);
            bw.Write(buf.Length);
            bw.Write(buf, 0, buf.Length);
        }

        public static string ReadUtf8String(this BinaryReader br)
        {
            var len = br.ReadInt32();
            var dataBuf = new byte[len];
            br.Read(dataBuf, 0, len);
            return Encoding.UTF8.GetString(dataBuf, 0, dataBuf.Length);
        }

        public static void WriteDateTime(this BinaryWriter bw, DateTime data)
        {
            var sec = (int) ((data.Ticks - UnixEpochBase)/10000000);
            var nsec = (int) ((data.Ticks%10000000)*100);

            bw.Write(sec);
            bw.Write(nsec);
        }

        public static DateTime ReadDateTime(this BinaryReader br)
        {
            var sec = br.ReadInt32();
            var nsec = br.ReadInt32();

            var ticks = ((long) (sec)*10000000 + (nsec)/100);
            return new DateTime(ticks + UnixEpochBase);
        }


        public static void WriteTimeSpan(this BinaryWriter bw, TimeSpan data)
        {
            var sec = (int) (data.Ticks/10000000);
            var nsec = (int) ((data.Ticks%10000000)*100);

            bw.Write(sec);
            bw.Write(nsec);
        }

        public static TimeSpan ReadTimeSpan(this BinaryReader br)
        {
            var sec = br.ReadInt32();
            var nsec = br.ReadInt32();

            var ticks = ((long) (sec)*10000000 + (nsec)/100);
            return new TimeSpan(ticks);
        }
    }
}