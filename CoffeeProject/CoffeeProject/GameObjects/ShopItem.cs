using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    public enum PlayerStat
    {
        Speed,
        Attack,
        Health
    }

    [SpriteSheet("healing_item")]
    public class ShopItem : Sprite, ICollisionChecker<Hero>, IMultiBehaviorComponent
    {
        private const float SineSpeed = 2f;
        private const float SineAmplitude = 25;
        public int Amount { get; set; } = 1;
        public int Cost { get; set; } = 1;
        public Label Label { get; set; }
        public PlayerStat Stat { get; set; } = PlayerStat.Health;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public ShopItem(IAnimationProvider provider) : base(provider)
        {
            var sineOffset = new OffsetFilter(Vector2.Zero);
            CombineWith(sineOffset);
            CombineWith(new TimeFunction(t => sineOffset.Offset = new Vector2(0, SineAmplitude * MathF.Sin(SineSpeed * (float)t))));
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            if (obj.Stats.Currency >= Cost)
            {
                obj.Stats.Currency -= Cost;
                Dispose();
                Label.Dispose();
                switch (Stat)
                {
                    case PlayerStat.Speed: obj.Stats.Speed += Amount; return;
                    case PlayerStat.Attack: obj.Stats.AttackPower += Amount; return;
                    case PlayerStat.Health: obj.Stats.HealthPower += Amount; return;
                }
            }
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with
        {
            OrderComparer = Position.ToPoint().Y,
            Scale = base.DisplayInfo.Scale * 0.3f
        };

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
        }
    }
}