using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [Box(20)]
    [SpriteSheet("heart")]
    internal class Heart : Sprite
    {
        public Heart(IAnimationProvider provider) : base(provider)
        {
        }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var value = base.DisplayInfo;
                value.Scale = new Microsoft.Xna.Framework.Vector2(0.4f, 0.4f);
                return value;
            }
        }
    }
}
