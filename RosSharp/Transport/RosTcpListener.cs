using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace RosSharp.Transport
{
    internal sealed class RosTcpListener : IDisposable
    {
        private Socket _socket;

        public RosTcpListener(int portNumber)
        {
            var hostEntry = new IPEndPoint(IPAddress.Any, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(hostEntry);
            _socket.Listen(50);
        }

        public bool Connected
        {
            get { return _socket.Connected; }
        }

        public IObservable<Socket> AcceptAsync()
        {
            return Observable.Create<Socket>(
                observer => _socket
                    .AcceptAsObservable(_socket.LocalEndPoint)
                    .Subscribe(observer));
        }

        public IPEndPoint EndPoint
        {
            get { return (IPEndPoint)_socket.LocalEndPoint; }
        }

        public void Dispose()
        {
            _socket.Close();
        }
    }
}
