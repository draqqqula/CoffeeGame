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

namespace MagicDustLibrary.Organization
{
    public class StateConnectionRecieveManager
    {
        private IGameClientProvider _clientProvider;
        private UdpClient _requestReciever;
        private Task<UdpReceiveResult> recieveTask;

        public Action<GameClient> OnConnected = delegate { };

        public void StartServer(int port)
        {
            _requestReciever = new UdpClient(port);
            recieveTask = _requestReciever.ReceiveAsync();
            recieveTask.ContinueWith(CheckResult);
        }
        public void StopServer()
        {
            _requestReciever.Close();
        }

        private void CheckResult(Task<UdpReceiveResult> task)
        {
            if (recieveTask.IsCompletedSuccessfully)
            {
                var client = _clientProvider.CreateClient(task.Result.RemoteEndPoint, task.Result.Buffer);
                OnConnected(client);
            }
            recieveTask = _requestReciever.ReceiveAsync();
            recieveTask.ContinueWith(CheckResult);
        }

        public StateConnectionRecieveManager(IGameClientProvider provider)
        {
            _clientProvider = provider;
        }

        public StateConnectionRecieveManager() : this(new DefaultClientProvider()) { }
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
