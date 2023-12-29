using CoffeeProject.Layers;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Logic;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.ComponentModel;

namespace CoffeeProject.BoxDisplay
{
    public static class BoxDisplayExtensions
    {
        public static T UseBoxDisplay<T>(this T body, IControllerProvider state, 
            Color goodColor, Color badColor, int gridSize) 
            where T : ComponentBase, IBodyComponent
        {
            body.CombineWith(state
                .Using<IFactoryController>()
                .CreateObject<BoxDisplay>()
                .SetPlacement(new Placement<BoxDisplayLayer>())
                .UseNewTexture(goodColor, badColor, gridSize)
                );
            return body;
        }
    }
}
