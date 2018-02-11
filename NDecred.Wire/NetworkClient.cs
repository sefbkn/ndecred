using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NDecred.Wire
{
    public interface INetworkClient : IDisposable
    {
        bool IsConnected { get; }
        int AvailableBytes { get; }
        BinaryReader GetStreamReader();
        BinaryWriter GetStreamWriter();
        Task ConnectAsync();
    }

    /// <summary>
    /// Abstracts TCP client and its underlying r/w streams.
    /// </summary>
    public class NetworkClient : INetworkClient
    {
        private readonly int _port;
        private readonly IPAddress _ipAddress;

        private bool _isDisposed;
        private bool _hasConnected;
        private readonly TcpClient _tcpClient;

        private BinaryReader _streamReader;
        private BinaryWriter _streamWriter;

        public NetworkClient(byte[] ip, int port)
        {
            _port = port;
            _ipAddress = new IPAddress(ip).MapToIPv4();

            _isDisposed = false;
            _hasConnected = false;
            _tcpClient = new TcpClient();
        }

        public bool IsConnected => _hasConnected && !_isDisposed;
        public int AvailableBytes => _isDisposed ? 0 : _tcpClient.Available;
        public BinaryReader GetStreamReader() => _streamReader = _streamReader ?? new BinaryReader(_tcpClient.GetStream(), Encoding.Default, true);
        public BinaryWriter GetStreamWriter() => _streamWriter = _streamWriter ?? new BinaryWriter(_tcpClient.GetStream(), Encoding.Default, true);

        /// <summary>
        /// Attempts to connect to the provided ip address and port.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            await _tcpClient.ConnectAsync(_ipAddress, _port);
            _hasConnected = true;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _hasConnected = false;
            _tcpClient.Dispose();
        }

        public override string ToString()
        {
            return $"{_ipAddress}:{_port}";
        }
    }
}