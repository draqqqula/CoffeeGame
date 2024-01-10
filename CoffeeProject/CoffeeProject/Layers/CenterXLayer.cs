using MagicDustLibrary.Display;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Layers
{
    [LayerPriority(254)]
    public class CenterXLayer : Layer
    {
        public override DrawingParameters Process(DrawingParameters arguments, GameCamera camera)
        {
            return arguments with { Position = new Microsoft.Xna.Framework.Vector2(camera.ViewPort.Center.X, arguments.Position.Y) };
        }
    }
}
