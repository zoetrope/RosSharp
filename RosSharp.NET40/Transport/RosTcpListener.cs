using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace RosSharp.Transport
{
    class RosTcpListener
    {
        private Socket _socket;

        public IObservable<Socket> AcceptAsync(int portNumber)
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
