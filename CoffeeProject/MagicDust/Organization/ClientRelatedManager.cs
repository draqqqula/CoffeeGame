using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MagicDustLibrary.Organization
{
    public abstract class ClientRelatedActions
    {
        public Action<GameClient> OnNewClient = delegate { };
        public Action<GameClient> OnDisposeClient = delegate { };
        public Action<GameClient> OnUpdateClient = delegate { };

        protected abstract void AddClient(GameClient client);
        protected abstract void RemoveClient(GameClient client);
        protected abstract void UpdateClient(GameClient client);

        public ClientRelatedActions()
        {
            OnNewClient += AddClient;
            OnDisposeClient += RemoveClient;
            OnUpdateClient += UpdateClient;
        }
    }
}
