using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RosSharp
{
    internal class SubscriberHeader
    {
        public string callerid { get; set; }
        public string topic { get; set; }
        public string md5sum { get; set; }
        public string type { get; set; }
    }

    internal class TcpRosHeaderSerializer<TDataType>
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
            throw new NotSupportedException();
        }
        
    }
}
