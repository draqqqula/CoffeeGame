using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Network
{
    public class MessageHandler : IDisposable
    {
        private readonly UdpClient _client;
        private Task<UdpReceiveResult>? _recieveTask;
        public void Start(Action<IPEndPoint, byte[]> handler)
        {
            HandleData = handler;
            _recieveTask = _client.ReceiveAsync();
            _recieveTask.ContinueWith(CheckForData);
        }

        public void CheckForData(Task<UdpReceiveResult> task)
        {
            if (_recieveTask is null)
            {
                return;
            }

            if (_recieveTask.IsCompletedSuccessfully)
            {
                HandleData(_recieveTask.Result.RemoteEndPoint, _recieveTask.Result.Buffer);
            }
            _recieveTask = _client.ReceiveAsync();
            _recieveTask.ContinueWith(CheckForData);
        }

        public void Dispose()
        {
            if (_recieveTask is null)
            {
                return;
            }
            _recieveTask.Dispose();
        }

        private Action<IPEndPoint, byte[]> HandleData =
            delegate { throw new Exception("Handler delegate must be set before running MessageHandler"); };

        public MessageHandler(UdpClient client)
        {
            _client = client;
        }
    }
}
