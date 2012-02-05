using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace RosSharp
{
    class TcpClient
    {
        private Socket _socket;
        
        public void Connect(string hostName, int portNumber)
        {
            var hostEntry = new DnsEndPoint(hostName, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var socketEventArg = new SocketAsyncEventArgs {RemoteEndPoint = hostEntry};

            Observable.FromEventPattern<SocketAsyncEventArgs>(
                e => socketEventArg.Completed += e,
                e => socketEventArg.Completed -= e);

            _socket.ConnectAsync(socketEventArg);

        }
    }
}
