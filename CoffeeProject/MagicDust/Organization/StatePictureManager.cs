using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.BaseServices;
using MagicDustLibrary.Organization.StateManagement;
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
        private readonly IEnumerable<Layer> _layers;
        private readonly IEnumerable<GameClient> _clients;
        private readonly CameraStorage _cameras;
        private readonly ViewStorage _viewPoints;

        public StatePictureManager(StateLayerManager layerManager, StateClientManager clientManager, 
            CameraStorage cameraStorage, ViewStorage viewStorage)
        {
            _layers = layerManager.GetAll();
            _clients = clientManager.GetAll();
            _cameras = cameraStorage;
            _viewPoints = viewStorage;
        }

        public void Update(IStateController controller, TimeSpan deltaTime, bool onPause)
        {
            foreach (var layer in _layers)
            {
                foreach (var displayProvider in layer)
                {
                    foreach (var client in _clients)
                    {
                        var camera = _cameras.GetFor(client);
                        var view = _viewPoints.GetFor(client);
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
