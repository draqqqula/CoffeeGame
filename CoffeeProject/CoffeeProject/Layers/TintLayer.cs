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
    [Paralax(0, 0)]
    internal class TintLayer : ParalaxLayer
    {
        public override DrawingParameters Process(DrawingParameters arguments, GameCamera camera)
        {
             return base.Process(arguments, camera);
        }
    }
}
