using MagicDustLibrary.Display;
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
using System.ComponentModel;
using System.Reflection;

namespace MagicDustLibrary.Organization.StateClientServices
{
    public class StateClientManager: IDisposable
    {
        private readonly List<GameClient> Clients = [];

        public event Action<GameClient> OnConnect = delegate { };
        public event Action<GameClient> OnUpdate = delegate { };
        public event Action<GameClient> OnDisconnect = delegate { };
        public void Connect(GameClient client)
        {
            if (!IsConnected(client))
            {
                Clients.Add(client);
                OnConnect(client);
                client.OnDispose += OnDisconnect;
                client.OnUpdate += OnUpdate;
            }
        }
        public void Disconnect(GameClient client)
        {
            Clients.Remove(client);
            OnDisconnect(client);
        }
        public bool IsConnected(GameClient client) => Clients.Contains(client);
        public IEnumerable<GameClient> GetAll()
        {
            return Clients.ToArray();
        }

        public void ConfigureRelated(ClientRelatedActions relatedManager)
        {
            OnConnect += relatedManager.OnNewClient;
            OnDisconnect += relatedManager.OnDisposeClient;
            OnUpdate += relatedManager.OnUpdateClient;
            foreach (var client in Clients)
            {
                relatedManager.OnNewClient(client);
            }
        }

        public void Dispose()
        {
            foreach (var client in Clients)
            {
                OnDisconnect(client);
                client.OnUpdate -= OnUpdate;
                client.OnDispose -= OnDisconnect;
            }
            Clients.Clear();
        }
    }
}
