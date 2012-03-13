using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RosSharp.Message;

namespace RosSharp.StdMsgs
{
    public class String : IMessage
    {
        public string MessageType
        {
            get { return "std_msgs/String"; }
        }

        public string Md5Sum
        {
            get { return "992ce8a1687cec8c8bd883ec73ca41d1"; }
        }

        public string MessageDefinition
        {
            get { return "string data"; }
        }

        public void Serialize(Stream stream)
        {
            var buf = Encoding.UTF8.GetBytes(data);

            stream.Write(BitConverter.GetBytes(buf.Length + 4), 0, 4);//TODO: これは外で付けたい。
            stream.Write(BitConverter.GetBytes(buf.Length), 0, 4);
            stream.Write(buf, 0, buf.Length);
        }

        public void Deserialize(Stream stream)
        {
            var dummy = new byte[4];
            stream.Read(dummy, 0, 4); //TODO: あとでなくす。

            var lenBuf = new byte[4];
            stream.Read(lenBuf, 0, 4);

            var len = BitConverter.ToInt32(lenBuf, 0);

            var dataBuf = new byte[len];
            stream.Read(dataBuf, 0, len);

            data = Encoding.UTF8.GetString(dataBuf, 0, dataBuf.Length);            
        }

        public string data { get; set; }
    }
}
