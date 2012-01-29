using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class MessageSerializationFactory
    {
        public MessageSerializer<TDataType> CreateMessageSerializer<TDataType>(string messageType)
        {
            return new MessageSerializer<TDataType>();
        }
    }
}
