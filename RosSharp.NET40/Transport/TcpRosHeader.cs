using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RosSharp.Transport
{
    internal class SubscriberHeader
    {
        public string callerid { get; set; }
        public string topic { get; set; }
        public string md5sum { get; set; }
        public string type { get; set; }
        public string message_definition { get; set; }
        public string latching { get; set; } // int?
    }
    internal class SubscriberResponseHeader
    {
        public string callerid { get; set; }
        public string topic { get; set; }
        public string md5sum { get; set; }
        public string type { get; set; }
        public string message_definition { get; set; }
        public string latching { get; set; } // int?
    }

    internal class ServiceHeader
    {
        public string callerid { get; set; }
        public string service { get; set; }
        public string md5sum { get; set; }
    }
    internal class ServiceResponseHeader
    {
        public string callerid { get; set; }
        public string service { get; set; }
        public string md5sum { get; set; }
        public string type { get; set; }
    }

    public class TcpRosHeaderSerializer<TDataType> where TDataType : new()
    {
        public void Serialize(Stream stream, TDataType data)
        {
            var list = typeof(TDataType)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => p.Name + "=" + p.GetValue(data, null))
                .ToList();

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

        public TDataType Deserialize(Stream stream)
        {
            var buf = new byte[4];
            stream.Read(buf, 0, 4);

            var length = BitConverter.ToInt32(buf, 0);
            
            
            var map = new Dictionary<string,string>();

            while (stream.Position < length+4)
            {
                var lenBuf = new byte[4];
                stream.Read(lenBuf, 0, 4);
                var len = BitConverter.ToInt32(lenBuf, 0);

                var dataBuf = new byte[len];
                stream.Read(dataBuf, 0, len);

                var data = Encoding.UTF8.GetString(dataBuf, 0, dataBuf.Length);
                var items = data.Split('=');

                map.Add(items[0],items[1]);
            }

            var ret = new TDataType();

            foreach (var v in map)
            {
                var p = typeof(TDataType).GetProperty(v.Key);
                p.SetValue(ret, v.Value, null);
            }

            return ret;
        }
        
    }
}
