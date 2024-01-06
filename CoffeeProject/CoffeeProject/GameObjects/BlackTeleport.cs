using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("black_teleport")]
    public class BlackTeleport : Sprite
    {
        public BlackTeleport(IAnimationProvider provider) : base(provider)
        {
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with 
        { 
            Scale = new Microsoft.Xna.Framework.Vector2(1f, 1f), 
            OrderComparer = Position.ToPoint().Y 
        };
    }
}
