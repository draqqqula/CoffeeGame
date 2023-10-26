using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class CameraStorage : ClientRelatedActions
    {
        private readonly CameraSettings _initialSettings;
        private readonly Dictionary<GameClient, GameCamera> _cameras = new();

        public GameCamera GetFor(GameClient client)
        {
            return _cameras[client];
        }

        protected override void AddClient(GameClient client)
        {
            _cameras.Add(client, new GameCamera(_initialSettings, client.Window));
        }

        protected override void RemoveClient(GameClient client)
        {
            _cameras[client].OnClientUpdated(client);
        }

        protected override void UpdateClient(GameClient client)
        {
            _cameras.Remove(client);
        }

        public CameraStorage(CameraSettings settings)
        {
            _initialSettings = settings;
        }
    }
}
