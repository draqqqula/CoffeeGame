using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StatePictureManager
    {
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
