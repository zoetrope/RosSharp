using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class Subscriber<TDataType> : ITopic, IObservable<TDataType>
    {
        public GraphName TopicName
        {
            get { throw new NotImplementedException(); }
        }

        public string TopicMessageType
        {
            get { throw new NotImplementedException(); }
        }

        public IDisposable Subscribe(IObserver<TDataType> observer)
        {
            throw new NotImplementedException();
        }
    }
}
