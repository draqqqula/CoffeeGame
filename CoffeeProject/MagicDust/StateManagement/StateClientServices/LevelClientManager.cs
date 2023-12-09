using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.StateClientServices
{
    public class LevelClientManager : ClientRelatedActions
    {
        private readonly GameLevel _level;
        public Action<IControllerProvider, GameClient> OnConnect = delegate { };
        public Action<IControllerProvider, GameClient> OnDisconnect = delegate { };
        public Action<IControllerProvider, GameClient> OnUpdate = delegate { };
        protected override void AddClient(GameClient client)
        {
            OnConnect(_level.GameState.Controller, client);
        }

        protected override void RemoveClient(GameClient client)
        {
            OnDisconnect(_level.GameState.Controller, client);
        }

        protected override void UpdateClient(GameClient client)
        {
            OnUpdate(_level.GameState.Controller, client);
        }

        public LevelClientManager(GameLevel level)
        {
            _level = level;
        }
    }
}
