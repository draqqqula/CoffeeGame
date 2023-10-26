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

namespace MagicDustLibrary.Organization
{
    public class StateClientManager
    {
        private readonly List<GameClient> Clients = new ();

        public Action<GameClient> OnConnect = delegate { };
        public Action<GameClient> OnUpdate = delegate { };
        public Action<GameClient> OnDisconnect = delegate { };
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
        public void ForceDisconnect(GameClient client)
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
    }
}
