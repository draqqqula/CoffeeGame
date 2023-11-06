using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
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
    [SpriteSheet("PauseTitle.aseprite")]
    public class PauseTitle : Sprite
    {
        public PauseTitle(IPlacement placement, Vector2 position, IAnimationProvider provider) : base(placement, position, provider)
        {
        }
    }
}
