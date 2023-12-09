using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using MagicDustLibrary.Organization.StateClientServices;

namespace MagicDustLibrary.Organization.DefualtImplementations
{
    public class CameraStorage : ClientRelatedActions, IUpdateService
    {
        private readonly CameraSettings _initialSettings;
        private readonly Dictionary<GameClient, GameCamera> _cameras = new();

        public bool RunOnPause => false;

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

        public void Update(IControllerProvider controller, TimeSpan deltaTime)
        {
            foreach (var camera in _cameras.Values)
            {
                camera.Update(deltaTime);
            }
        }

        public CameraStorage(CameraSettings settings)
        {
            _initialSettings = settings;
        }
    }
}
