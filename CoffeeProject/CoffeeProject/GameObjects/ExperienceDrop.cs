using BehaviorKit;
using CoffeeProject.Behaviors;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("soul")]
    public class ExperienceDrop : Sprite, IMultiBehaviorComponent
    {
        public int Amount { get; set; } = 1;
        private const float SineSpeed = 2f;
        private const float SineAmplitude = 25;
        private const float StartSpeed = 1f;
        private const float FollowAcceleration = 2f;
        public ExperienceDrop(IAnimationProvider provider) : base(provider)
        {
            CombineWith(new OffsetFilter(new Microsoft.Xna.Framework.Vector2(0, -30)));
            CombineWith(new Physics(new SurfaceMapping.SurfaceMap([], 0, 14)));
            var sineOffset = new OffsetFilter(Microsoft.Xna.Framework.Vector2.Zero);
            CombineWith(sineOffset);
            CombineWith(new TimeFunction(t => sineOffset.Offset = new Vector2(0, SineAmplitude * MathF.Sin(SineSpeed * (float)t))));
            CombineWith(new TimerHandler());
        }

        public IBodyComponent Target { get; set; }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
            if (Target is null)
            {
                return;
            }
            var physics = GetComponents<Physics>().First();
            var timer = GetComponents<TimerHandler>().First();
            var direction = Target.Position - Position;
            if (physics.ActiveVectors.ContainsKey("follow"))
            {
                physics.DirectVector("follow", direction);
            }
            else
            {
                direction.Normalize();
                physics.AddVector("follow", new MovementVector(StartSpeed * direction, FollowAcceleration, TimeSpan.FromSeconds(1), true));
            }

            if ((Position - Target.Position).Length() < 10)
            {
                Dispose();
                if (Target is Hero hero)
                {
                    hero.Stats.Currency += Amount;
                }
            }
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with
        {
            OrderComparer = Position.ToPoint().Y,
            Scale = new Microsoft.Xna.Framework.Vector2(0.1f, 0.1f)
        };
    }
}
