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
            var sec = (int)((data.Ticks - UnixEpochBase) / 10000000);
            var nsec = (int)((data.Ticks % 10000000) * 100);

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
            var sec = (int)(data.Ticks/10000000);
            var nsec = (int)((data.Ticks % 10000000) * 100);

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
