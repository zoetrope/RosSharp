using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RosSharp
{
    public class MessageSerializer<TDataType> where TDataType : IMessage, new ()
    {
        public void Serialize(Stream stream, TDataType data)
        {
            // TODO: 

            var v = typeof(TDataType)
                .GetProperty("Data")
                .GetValue(data,null);

            var buf = Encoding.UTF8.GetBytes((string)v);

            stream.Write(BitConverter.GetBytes(buf.Length + 4), 0, 4);
            stream.Write(BitConverter.GetBytes(buf.Length), 0, 4);
            stream.Write(buf, 0, buf.Length);

        }

        public TDataType Deserialize(Stream stream)
        {
            var buf = new byte[4];
            stream.Read(buf, 0, 4);

            var length = BitConverter.ToInt32(buf, 0);


            var set = new HashSet<string>();

            while (stream.Position < length + 4)
            {
                var lenBuf = new byte[4];
                stream.Read(lenBuf, 0, 4);
                var len = BitConverter.ToInt32(lenBuf, 0);

                var dataBuf = new byte[len];
                stream.Read(dataBuf, 0, len);

                var data = Encoding.UTF8.GetString(dataBuf, 0, dataBuf.Length);
                set.Add(data);
            }

            var ret = new TDataType();


            var p = typeof (TDataType).GetProperty("Data");
            p.SetValue(ret, set.First(), null);


            return ret;
        }

    }
}
