using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StatePictureManager : IUpdateService
    {
        private readonly StateClientManager _stateClientManager;
        private readonly StateLayerManager _stateLayerManager;
        private readonly CameraStorage _cameraStorage;
        private readonly ViewStorage _viewStorage;
        private readonly IContentStorage _contentStorage;

        public StatePictureManager(StateClientManager clientManager, StateLayerManager layerManager,
    CameraStorage cameraStorage, ViewStorage viewStorage, IContentStorage contentStorage)
        {
            _stateClientManager = clientManager;
            _stateLayerManager = layerManager;
            _cameraStorage = cameraStorage;
            _viewStorage = viewStorage;
            _contentStorage = contentStorage;
        }

        public bool RunOnPause => true;

        public void Update(IStateController controller, TimeSpan deltaTime)
        {
            UpdatePicture(_stateLayerManager.GetAll(), _stateClientManager.GetAll(), _cameraStorage, _viewStorage);
        }

        public void UpdatePicture(IEnumerable<Layer> layers, IEnumerable<GameClient> clients, CameraStorage cameras, ViewStorage viewPoints)
        {
            foreach (var layer in layers)
            {
                foreach (var displayProvider in layer)
                {
                    foreach (var client in clients)
                    {
                        var camera = cameras.GetFor(client);
                        var view = viewPoints.GetFor(client);
                        var displayables = displayProvider.GetDisplay(camera, layer);
                        foreach (var displayable in displayables)
                        {
                            view.Add(displayable);
                        }
                    }
                }
            }
        }
    }
}
