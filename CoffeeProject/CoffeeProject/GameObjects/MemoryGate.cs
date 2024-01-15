using BehaviorKit;
using CoffeeProject.Behaviors;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("soul")]
    public class MemoryGate : Sprite, IMultiBehaviorComponent
    {
        private const float SineSpeed = 2f;
        private const float SineAmplitude = 25;
        private const float StartSpeed = 0.1f;
        private const float FollowAcceleration = 0.1f;
        public MemoryGate(IAnimationProvider provider) : base(provider)
        {
            CombineWith(new OffsetFilter(new Microsoft.Xna.Framework.Vector2(0, -150)));
            CombineWith(new Physics(new SurfaceMapping.SurfaceMap([], 0, 14)));
            var sineOffset = new OffsetFilter(Microsoft.Xna.Framework.Vector2.Zero);
            CombineWith(sineOffset);
            CombineWith(new TimeFunction(t => sineOffset.Offset = new Vector2(0, SineAmplitude * MathF.Sin(SineSpeed * (float)t))));
            CombineWith(new VisualShake());
            CombineWith(new TimerHandler());
        }

        public IBodyComponent Target { get; set; }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
            if ( Target is null)
            {
                return;
            }
            var physics = GetComponents<Physics>().First();
            var shake = GetComponents<VisualShake>().First();
            var timer = GetComponents<TimerHandler>().First();
            var direction = Target.Position - Position;
            if ( physics.ActiveVectors.ContainsKey("follow"))
            {
                physics.DirectVector("follow", direction);
                var interval = Math.Max(Math.Pow(direction.Length() / 3, 0.75) / 20, 0.1);
                if (timer.OnLoop("shake", TimeSpan.FromSeconds(interval), delegate { }))
                {
                    state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "soulbeat"), "sound").Play();
                    shake.Start(8, TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(Math.Max(interval - 0.14, 0.08)), 1, 0);
                }
            }
            else
            {
                direction.Normalize();
                physics.AddVector("follow", new MovementVector(StartSpeed * direction, FollowAcceleration, TimeSpan.FromSeconds(1), true));
            }

            if ((Position - Target.Position).Length() < 1)
            {
                Dispose();
                if (Target is Hero hero)
                {
                    var newArgs = new List<string>();
                    var oldArgs = hero.LevelArgs;
                    foreach (var arg in oldArgs.Data)
                    {
                        newArgs.Add(arg);
                    }
                    newArgs.Add(JsonSerializer.Serialize(hero.Stats));
                    state.Using<ILevelController>().LaunchLevel("memory", new LevelArgs(newArgs.ToArray()), false);
                }
            }
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with 
        { 
            OrderComparer = Position.ToPoint().Y, 
            Scale = new Microsoft.Xna.Framework.Vector2(0.25f, 0.25f)
        };
    }
}
