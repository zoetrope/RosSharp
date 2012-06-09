using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosSharp
{
    public interface IAsyncDisposable : IDisposable
    {
        /// <summary>
        /// Asynchronous Dispose
        /// </summary>
        /// <returns>task object for asynchronous operation</returns>
        Task DisposeAsync();

        event Func<string, Task> Disposing;
    }
}
