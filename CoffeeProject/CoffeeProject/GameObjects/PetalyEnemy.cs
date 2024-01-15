using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.Animations;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.ComponentModel;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.CommonObjectTypes.Image;
using MagicDustLibrary.Factorys;
using CoffeeProject.Layers;

namespace CoffeeProject.GameObjects
{
    [MoveDuration(3)]
    [MoveFreeSpan(0.1f)]
    public class PetalyIdle : UnitMove<PetalyEnemy, GameObject>
    {
        public override int GetAttraction(PetalyEnemy unit, GameObject target)
        {
            return 1;
        }
        public override void OnStart(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
        }
        public override bool Continue(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            var timer = unit.GetComponents<TimerHandler>().First();
            physics.ActiveVectors.Clear();
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (targetPosition.X > unit.Position.X)
            {
                unit.Animator.SetAnimation("Right", 0);
            }
            else
            {
                unit.Animator.SetAnimation("Left", 0);
            }

            if ((targetPosition - unit.Position).Length() < 600)
            {
                timer.OnLoop("attack", TimeSpan.FromSeconds(0.7), () =>
                {
                    PetalyAttack.CreateAttack(state,
                    target.GetComponents<IBodyComponent>().First().Position +
                    target.GetComponents<Physics>().First().GetResultingVector(TimeSpan.FromSeconds(0.2)), unit.GetComponents<Dummy>().First(), unit.Level);
                    unit.InvokeAttack(state, target);
                });
            }
            return true;
        }

        public override void OnEnd(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
        }

        public override void OnForcedBreak(IControllerProvider state, PetalyEnemy unit, GameObject target, UnitMove<PetalyEnemy, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [MoveStepsRequired(3)]
    [EndlessMove]
    [NoFreeSpanMove]
    [MovePriority(1)]
    public class PetalyEscape : UnitMove<PetalyEnemy, GameObject>
    {
        public override int GetAttraction(PetalyEnemy unit, GameObject target)
        {
            return 2;
        }

        public override bool Continue(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            return unit.Animator.Running.Name == "Escape";
        }

        public override void OnStart(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            unit.Animator.SetAnimation("Escape", 0);
        }
    }

    [MoveStepsRequired(3)]
    [EndlessMove]
    [NoFreeSpanMove]
    [MovePriority(3)]
    public class PetalyAppear : UnitMove<PetalyEnemy, GameObject>
    {
        public override int GetAttraction(PetalyEnemy unit, GameObject target)
        {
            return 2;
        }

        public override bool Continue(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            return unit.Animator.Running.Name == "Appear";
        }

        public override void OnStart(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            unit.Animator.SetAnimation("Appear", 0);
        }
    }

    [MoveStepsRequired(3)]
    [MoveDuration(2)]
    [MoveFreeSpan(0.1)]
    [MovePriority(2)]
    public class PetalyMoveUnderground : UnitMove<PetalyEnemy, GameObject>
    {
        private const float Speed = 4;
        private const float ParticleAreaSize = 15f;
        private const float ParticleLifeTimeMin = 0.5f;
        private const float ParticleLifeTimeMax = 1.3f;
        private const double ParticleCreationDelay = 0.1f;
        private const int ParticlePerTime = 4;
        private const int ParticleMinSize = 5;
        private const int ParticleMaxSize = 10;
        public override int GetAttraction(PetalyEnemy unit, GameObject target)
        {
            return 2;
        }

        public override bool Continue(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            var unitTimer = unit.GetComponents<TimerHandler>().First();
            var random = new RandomEx();

            unitTimer.OnLoop("createParticle", TimeSpan.FromSeconds(ParticleCreationDelay), () =>
            {
                for (var i = 0; i < ParticlePerTime; i++)
                {
                    CreateParticle(state, unit, random);
                }
            });

            return true;
        }

        private void CreateParticle(IControllerProvider state, PetalyEnemy unit, RandomEx random)
        {
            var timer = new TimerHandler();
            var size = ParticleMinSize + random.Next(ParticleMaxSize - ParticleMinSize);
            var particle = state.Using<IFactoryController>()
                .CreateObject<Image>()
                .SetTexture("ground_particle")
                .SetPos(unit.Position + new Vector2(random.NextSingle(-1, 1, 1), random.NextSingle(-1, 1, 1)) * ParticleAreaSize)
                .SetPlacement(new Placement<MainLayer>())
                .SetScale(new Vector2(0.2f, 0.2f) * size)
                .SetBounds(new Rectangle(-2, -2, 4, 4))
                .AddComponent(timer)
                .AddToState(state);
            timer.SetTimer("dispose", TimeSpan.FromSeconds(random.NextSingle(ParticleLifeTimeMin, ParticleLifeTimeMax, 1)), particle.Dispose, true);
        }

        public override void OnStart(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            var body = target.GetComponents<IBodyComponent>().First();
            var dummy = unit.GetComponents<Dummy>().First();

            dummy.IsInvincible = true;
            var random = new RandomEx();
            var deltaPosition = body.Position - unit.Position;
            var deltaAngle = MathEx.VectorToAngle(deltaPosition);
            var angleOffset = random.NextSingle(-MathF.PI / 4, MathF.PI / 4, 1);
            var direction = MathEx.AngleToVector(MathEx.NormalizeAngle(deltaAngle + angleOffset, MathF.PI * 2));
            physics.AddVector("move", new MovementVector(Speed * direction, 0, TimeSpan.FromSeconds(1), true));
            unit.Animator.SetAnimation("Default", 0);
        }

        public override void OnEnd(IControllerProvider state, PetalyEnemy unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.RemoveVector("move");
            var dummy = unit.GetComponents<Dummy>().First();
            dummy.IsInvincible = false;
        }

        public override void OnForcedBreak(IControllerProvider state, PetalyEnemy unit, GameObject target, UnitMove<PetalyEnemy, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [SpriteSheet("petaly")]
    public class PetalyEnemy : Sprite, IEnemy, IMultiBehaviorComponent, IUpdateComponent
    {
        public int Level { get; set; } = 1;
        public PetalyEnemy(IAnimationProvider provider) : base(provider)
        {
            this.CombineWith(
    new Physics(
    new SurfaceMap([], 0, 1)
    ));
            var unit = new Unit<PetalyEnemy, GameObject>();
            this.CombineWith(
                new TimerHandler());
            this.CombineWith(
                unit
                );
            unit.AddAction("idle", new PetalyIdle());
            unit.AddAction("escape", new PetalyEscape());
            unit.AddAction("appear", new PetalyAppear());
            unit.AddAction("move", new PetalyMoveUnderground());
        }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = info.Scale * new Vector2(0.23f, 0.23f);
                info.OrderComparer = Position.ToPoint().Y;
                return info;
            }
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };
        public event Action<IControllerProvider, GameObject> OnAttack = delegate { };
        public void InvokeAttack(IControllerProvider state, GameObject target)
        {
            OnAttack(state, target);
        }
        public void SetTarget(IControllerProvider state, GameObject target)
        {
            this.InvokeEach<Unit<PetalyEnemy, GameObject>>(it => it.SetTarget(state, target, this));
        }

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);

            var dummy = GetComponents<Dummy>().First();

            if (!dummy.IsAlive)
            {
                var memory = state.Using<IFactoryController>()
                .CreateObject<ExperienceDrop>()
                .SetPlacement(new Placement<MainLayer>())
                .SetPos(Position)
                .AddShadow(state)
                .SetBounds(new Rectangle(-7, -15, 15, 15))
                .AddToState(state);
                var target = GetComponents<Unit<PetalyEnemy, GameObject>>().First().Target;
                if (target is null)
                {
                    return;
                }
                memory.Target = target.GetComponents<IBodyComponent>().First();
                memory.Amount = 15 + Level * 5;
                Dispose();
            }
        }
    }
}
