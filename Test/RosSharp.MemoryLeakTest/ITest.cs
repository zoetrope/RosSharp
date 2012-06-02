using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.MemoryLeakTest
{
    interface ITest
    {
        void Initialize();
        void Do(int index);
        void Cleanup();
    }
}
