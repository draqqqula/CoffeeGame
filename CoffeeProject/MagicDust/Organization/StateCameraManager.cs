using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;

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
            _cameras.Remove(client);
        }

        protected override void UpdateClient(GameClient client)
        {
            _cameras[client].OnClientUpdated(client);
        }

        public CameraStorage(CameraSettings settings)
        {
            _initialSettings = settings;
        }
    }
}
