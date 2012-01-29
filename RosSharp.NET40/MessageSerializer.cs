using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class MessageSerializer<TDataType>
    {
        public void Serialize(Stream stream, TDataType data)
        {
            
        }

        public TDataType Deserialize(Stream stream)
        {
            throw new NotSupportedException();
        }

    }
}
