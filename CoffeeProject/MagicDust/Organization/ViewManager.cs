using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class ViewStorage : ClientRelatedActions
    {
        private readonly Dictionary<GameClient, GameView> _viewPoints = new();

        public GameView GetFor(GameClient client)
        {
            return _viewPoints[client];
        }

        protected override void AddClient(GameClient client)
        {
            _viewPoints.Add(client, new GameView());
        }

        protected override void RemoveClient(GameClient client)
        {
        }

        protected override void UpdateClient(GameClient client)
        {
            _viewPoints.Remove(client);
        }
    }
}
