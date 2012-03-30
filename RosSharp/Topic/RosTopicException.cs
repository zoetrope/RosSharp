using System;
using System.Runtime.Serialization;

namespace RosSharp.Topic
{
    public class RosTopicException : Exception
    {
        public RosTopicException()
        {
            
        }

        public RosTopicException(string message)
            :base(message)
        {
        }

        public RosTopicException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RosTopicException(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
        }
    }
}
