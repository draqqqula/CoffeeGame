using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [Box(10)]
    [SpriteSheet("panel-menu")]
    public class PausePanel : Sprite
    {
        public PausePanel(IAnimationProvider provider) : base(provider)
        {
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { Scale = new Vector2(0.2f, 0.2f) };
    }
}
