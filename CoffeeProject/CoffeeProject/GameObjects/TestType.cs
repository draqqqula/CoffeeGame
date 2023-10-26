using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [Box(10)]
    [SpriteSheet("hero")]
    public class TestType : Sprite
    {
        public GameClient Client;
        public TestType(IPlacement placement, Vector2 position, IAnimationProvider provider) : base(placement, position, provider)
        {
        }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = new Vector2(0.1f, 0.1f);
                return info;
            }
        }

        public override void OnTick(IStateController state, TimeSpan deltaTime)
        {
            if (Client.Controls[Control.left])
            {
                SetPosition(GetPosition() + new Vector2(1, 0));
            }
        }
    }
}
