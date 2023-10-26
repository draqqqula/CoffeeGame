using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class LevelClientManager : ClientRelatedActions
    {
        private readonly GameLevel _level;
        public Action<IStateController, GameClient> OnConnect = delegate { };
        public Action<IStateController, GameClient> OnDisconnect = delegate { };
        public Action<IStateController, GameClient> OnUpdate = delegate { };
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
