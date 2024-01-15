using BehaviorKit;
using CoffeeProject.Collision;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.CustomNodes
{
    public class FireSpawner : Behavior<IBodyComponent>
    {
        private float MaxDistance = 40;
        private float DistanceFactor = 0.5f;
        private TimerHandler Timer {  get; set; }
        public FireSpawner()
        {
            AddGreetingFor<TimerHandler>(it => Timer = it);
        }
        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IBodyComponent parent)
        {
            Timer.OnLoop("CreateFire", TimeSpan.FromSeconds(0.3), () =>
            {
                var random = new RandomEx();
                state.Using<IFactoryController>().CreateObject<Fire>()
                .SetPos(parent.Position + random.NextSingle(0, MaxDistance, DistanceFactor) * MathEx.AngleToVector(random.NextSingle(0, 2 * MathF.PI, 1)))
                .SetPlacement(new Placement<MainLayer>())
                .AddToState(state);
            });
        }
    }

    [SpriteSheet("fire")]
    public class Fire : Sprite, ICollisionChecker<Hero>
    {
        private readonly DamageInstance _damage;
        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            if (t > StartDoingDamage && t < StopDoingDamage)
            {
                obj.GetComponents<Dummy>().First().TakeDamage(_damage);
            }
        }

        private double t = 0;
        private double LifeTime = 3;
        private float ScaleFactor = 0.5f;
        private float Scale = 0;
        private float StartFade = 1;
        private double StartDoingDamage = 0.5;
        private double StopDoingDamage = 2;
        public Fire(IAnimationProvider provider) : base(provider)
        {
            _damage = new DamageInstance(new Dictionary<DamageType, int>() { { DamageType.Fire, 1 } }, Team.enemy, [], "Fire", null, [], [], TimeSpan.FromSeconds(0.3));
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { OrderComparer = Position.ToPoint().Y, Scale = Vector2.One * Scale };

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            t += deltaTime.TotalSeconds;
            if ( t > LifeTime )
            {
                Dispose();
            }

            if ( t < StartFade )
            {
                Scale = MathF.Pow(Convert.ToSingle(t / StartFade), ScaleFactor);
            }
            else
            {
                Scale = MathF.Pow(1 - Convert.ToSingle(t / (LifeTime - StartFade)), ScaleFactor);
            }
        }
    }
}
