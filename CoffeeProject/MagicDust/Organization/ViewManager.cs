using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.BaseServices;
using MagicDustLibrary.Organization.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class ViewStorage : ClientRelatedActions
    {
        private readonly Dictionary<GameClient, ViewBuffer> _viewPoints = new();

        public StateServiceCollection Services { get; set; }

        public ViewBuffer GetFor(GameClient client)
        {
            return _viewPoints[client];
        }

        protected override void AddClient(GameClient client)
        {
            _viewPoints.Add(client, new ViewBuffer());
        }

        protected override void RemoveClient(GameClient client)
        {
            _viewPoints.Remove(client);
        }

        protected override void UpdateClient(GameClient client)
        {
        }
    }
}
