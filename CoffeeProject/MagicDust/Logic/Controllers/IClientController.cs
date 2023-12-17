using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.StateClientServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface IClientController : IStateController
    {
        public void OpenServer(int port);
        public void TransferClient(GameClient client, string targetLevel);
        public void Disconnect(GameClient client);
        public void AttachCamera<T>(GameClient client, T obj) where T : IBodyComponent;
    }

    internal class DefaultClientController : IClientController
    {
        private readonly StateLevelManager _levelManager;
        private readonly StateClientManager _clientManager;
        private readonly StateConnectionRecieveManager _recieveManager;
        private readonly CameraStorage _cameraStorage;
        public DefaultClientController(
            StateLevelManager levelManager, 
            StateClientManager clientManager, 
            StateConnectionRecieveManager recieveManager,
            CameraStorage cameraStorage
            )
        {
            _levelManager = levelManager;
            _clientManager = clientManager;
            _recieveManager = recieveManager;
            _cameraStorage = cameraStorage;
        }

        public void OpenServer(int port)
        {
            _recieveManager.StartServer(port);
        }

        public void TransferClient(GameClient client, string targetLevel)
        {
            var targetLevelState = _levelManager.ApplicationLevelManager.GetLevel(targetLevel).GameState;
            targetLevelState.GetProvider().GetService<StateClientManager>().Connect(client);
        }

        public void Disconnect(GameClient client)
        {
            _clientManager.Disconnect(client);
        }

        public void AttachCamera<T>(GameClient client, T obj) where T : IBodyComponent
        {
            _cameraStorage.GetFor(client).LinkTo(obj);
        }
    }
}
