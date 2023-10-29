using CoffeeProject.Family;
using CoffeeProject.Layers;
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
    [Box(50, 50, 25, 25)]
    [SpriteSheet("Painting")]
    [MemberShip<Global>]
    public class TestType2 : Sprite
    {
        public GameClient Client { get; set; }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = new Vector2(0.01f, 0.01f);
                return info;
            }
        }
        public override void OnTick(IStateController state, TimeSpan deltaTime)
        {

        }

        public TestType2(IPlacement placement, Vector2 position, IAnimationProvider provider) : base(placement, position, provider)
        {
        }
    }
}
