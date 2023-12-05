using MagicDustLibrary.Logic;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Network;

namespace MagicDustLibrary.Organization
{
    public class StateConnectionRecieveManager
    {
        private IGameClientProvider _clientProvider;
        private UdpClient _requestReciever;
        private MessageHandler _handler;
        private readonly object _lock;

        public event Action<GameClient> OnConnected = delegate { };

        public void StartServer(int port)
        {
            _requestReciever = new UdpClient(port);
            _handler = new MessageHandler(_requestReciever);
            _handler.Start(HandleConnection);
        }
        public void StopServer()
        {
            _requestReciever.Close();
        }

        private void HandleConnection(IPEndPoint host, byte[] data)
        {
            lock (_lock)
            {
                var client = _clientProvider.CreateClient(host, data);
                OnConnected(client);
            }
        }

        public StateConnectionRecieveManager(object lockObject, IGameClientProvider provider)
        {
            _clientProvider = provider;
            _lock = lockObject;
        }

        public StateConnectionRecieveManager(object lockObject) : this(lockObject, new DefaultClientProvider()) { }
    }

    public interface IGameClientProvider
    {
        public GameClient CreateClient(IPEndPoint remoteHost, byte[] initialPack);
    }

    public class DefaultClientProvider : IGameClientProvider
    {
        public GameClient CreateClient(IPEndPoint remoteHost, byte[] initialPack)
        {
            GameControls controls = new();
            ReadOnlySpan<byte> bytes = initialPack.AsSpan();
            Rectangle window = new Rectangle(
                BinaryPrimitives.ReadInt32LittleEndian(bytes),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[4..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[8..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[12..])
                );
            var client = new GameClient(window, controls, GameClient.GameLanguage.Russian, remoteHost);
            return client;
        }
    }
}
