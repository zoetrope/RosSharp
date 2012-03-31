using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace RosSharp.Transport
{
    internal sealed class TcpRosClient : IDisposable
    {
        private Socket _socket;

        public TcpRosClient()
        {
        }

        public TcpRosClient(Socket socket)
        {
            _socket = socket;
        }

        public void Dispose()
        {
            _socket.Close();
        }

        public bool Connected
        {
            get { return _socket.Connected; }
        }

        public Task ConnectTaskAsync(string hostName, int portNumber)
        {
            var hostEntry = new DnsEndPoint(hostName, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            return _socket.ConnectTaskAsync(hostEntry);
        }

        public Task<int> SendTaskAsync(byte[] data)
        {
            return _socket.SendTaskAsync(data);
        }

        private IConnectableObservable<SocketAsyncEventArgs> _receiver;

        public IObservable<byte[]> ReceiveAsync(int offset = 0)
        {
            if (_receiver == null)
            {
                _receiver = _socket.ReceiveAsObservable().Publish();
                _receiver.Connect(); //TODO: connectのタイミングはここでよいか？
            }

            return Observable.Create<byte[]>(observer =>
            {
                var disposable = _receiver
                    .Select(OnReceive)
                    .Scan(new byte[] { }, (abs, bs) =>
                    {
                        var rest = AppendData(abs, bs);
                        byte[] current;
                        if (CompleteMessage(offset, out current, ref rest))
                        {
                            observer.OnNext(current);
                        }

                        return rest;
                    })
                    .Subscribe(_ => { },
                               ex =>
                               {
                                   Console.WriteLine("ReceiveAsync Error = {0}", ex.Message);
                                   //_socket.Close();//TODO: 閉じなくてよい？
                                   observer.OnError(new Exception());
                                   observer.OnCompleted();
                               });

                return disposable;
            });
        }


        private byte[] OnReceive(SocketAsyncEventArgs args)
        {
            var ret = new byte[args.BytesTransferred];
            Buffer.BlockCopy(args.Buffer, 0, ret, 0, args.BytesTransferred);

            return ret;
        }

        private byte[] AppendData(byte[] bs1, byte[] bs2)
        {
            var rs = new byte[bs1.Length + bs2.Length];
            bs1.CopyTo(rs, 0);
            bs2.CopyTo(rs, bs1.Length);
            return rs;
        }
        private bool CompleteMessage(int offset, out byte[] current, ref byte[] rest)
        {
            if (rest.Length == 0)
            {
                current = new byte[0];
                return false;
            }

            if (rest.Length < 4 + offset)
            {
                current = new byte[0];
                return false;
            }


            var length = BitConverter.ToInt32(rest, offset) + 4;

            if (rest.Length < length + offset)
            {
                current = new byte[0];
                return false;
            }

            current = new byte[length];
            Buffer.BlockCopy(rest, offset, current, 0, length);

            var restLen = rest.Length - offset - length;
            var temp = new byte[restLen];
            Buffer.BlockCopy(rest, length, rest, 0, restLen);
            rest = temp;

            return true;
        }
    }
}
