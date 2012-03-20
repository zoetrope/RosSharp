using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if !WINDOWS_PHONE
using System.Dynamic;
#endif

namespace RosSharp.Transport
{
#if WINDOWS_PHONE
    internal sealed class TcpRosHeader
    {
        private Dictionary<string, string> _members;
        public TcpRosHeader(Dictionary<string,string> members)
        {
            _members = members;
        }
        
        public string GetValue(string key)
        {
            return _members[key];
        }
    }
#else
    internal sealed class TcpRosHeader : DynamicObject
    {
        private Dictionary<string, string> _members;
        public TcpRosHeader(Dictionary<string,string> members)
        {
            _members = members;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = string.Empty;
            if(!_members.ContainsKey(binder.Name))
            {
                return false;
            }

            result = _members[binder.Name];
            return true;
        }
    }
#endif

    internal static class TcpRosHeaderSerializer
    {
        public static void Serialize(Stream stream, object data)
        {
            var list = data.GetType()
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

        public static TcpRosHeader Deserialize(Stream stream)
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

            return new TcpRosHeader(map);
        }
        
    }
}
