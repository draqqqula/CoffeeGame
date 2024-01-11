using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.Logic.Behaviors;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("soul")]
    internal class BossSpawner : Sprite, IMultiBehaviorComponent
    {
        public Hero Target { get; set; }
        private bool IsActivated {  get; set; } = false;
        private const float SineSpeed = 2f;
        private const float SineAmplitude = 25;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public BossSpawner(IAnimationProvider provider) : base(provider)
        {
            CombineWith(new OffsetFilter(new Vector2(0, -150)));
            var dummy = new Dummy(1, [], Team.enemy, [], [], 1);
            var sineOffset = new OffsetFilter(Vector2.Zero);
            CombineWith(sineOffset);
            CombineWith(dummy);
            CombineWith(new TimeFunction(t => sineOffset.Offset = new Vector2(0, SineAmplitude * MathF.Sin(SineSpeed * (float)t))));
            CombineWith(new VisualShake());
            CombineWith(new TimerHandler());
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with
        {
            OrderComparer = Position.ToPoint().Y,
            Scale = new Microsoft.Xna.Framework.Vector2(0.25f, 0.25f),
            Color = Color.Black
        };

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            OnAct(state, deltaTime, this);
            var dummy = GetComponents<Dummy>().First();
            var timer = GetComponents<TimerHandler>().First();
            var shake = GetComponents<VisualShake>().First();

            if (!dummy.IsAlive && !IsActivated)
            {
                IsActivated = true;
                timer.SetTimer("activation", 3, false);
                shake.Start(3, TimeSpan.FromSeconds(0.3), TimeSpan.FromSeconds(0.15), 40, -2);
                return;
            }

            if (IsActivated && timer.Check("activation") == TimerState.IsOut)
            {
                SummonBoss(state);
                Dispose();
            }
        }

        public void SummonBoss(IControllerProvider state)
        {
            var boss = state.Using<IFactoryController>()
                .CreateObject<Demon>()
                .SetPos(Position)
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddShadow(state)
                .AddToState(state);
            boss.InvokeEach<Physics>(it => it.SurfaceMap = state.Using<SurfaceMapProvider>().GetMap("level"));
            boss.SetTarget(state, Target);
        }
    }
}
