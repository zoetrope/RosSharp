using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace RosSharp
{
    class RosTcpListener
    {
        private Socket _socket;

        public IObservable<Socket> Start(int portNumber)
        {
            var hostEntry = new IPEndPoint(IPAddress.Any, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(hostEntry);
            _socket.Listen(50);

            return Observable.Create<Socket>(
                observer => _socket
                    .AcceptAsObservable(hostEntry)
                    .Select(x => x.AcceptSocket)
                    .Subscribe(observer));

        }
    }
}
