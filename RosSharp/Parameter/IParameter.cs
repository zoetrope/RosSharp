using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Parameter
{
    internal interface IParameter : IDisposable
    {
        void Update(object value);
    }
}
