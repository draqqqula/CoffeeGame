using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.DefualtImplementations
{
    public class DefaultDisplayService : IDisplayService
    {
        private readonly ViewStorage _viewStorage;
        private readonly IContentStorage _contentStorage;
        private readonly CameraStorage _cameraStorage;
        public DefaultDisplayService(ViewStorage viewStorage, IContentStorage contentStorage, CameraStorage cameraStorage)
        {
            _viewStorage = viewStorage;
            _contentStorage = contentStorage;
            _cameraStorage = cameraStorage;
        }

        public void Draw(GameClient client, SpriteBatch batch)
        {
            foreach (var displayable in _viewStorage.GetFor(client).GetAndClear())
            {
                displayable.Draw(batch,
                    _cameraStorage.GetFor(client),
                    _contentStorage);
            }
        }
    }
}
