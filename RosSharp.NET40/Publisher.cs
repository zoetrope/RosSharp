using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class Publisher<TDataType> : ITopic, IObserver<TDataType>
    {
        public GraphName TopicName
        {
            get { throw new NotImplementedException(); }
        }

        public string TopicMessageType
        {
            get { throw new NotImplementedException(); }
        }

        public void OnNext(TDataType value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
