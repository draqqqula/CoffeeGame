using MagicDustLibrary.Display;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Layers
{
    [LayerPriority(255)]
    internal class CenterLayer : Layer
    {
        public override DrawingParameters Process(DrawingParameters arguments, GameCamera camera)
        {
            arguments.Position = camera.ClientBounds.Center.ToVector2();
            return arguments;
        }
    }
}
