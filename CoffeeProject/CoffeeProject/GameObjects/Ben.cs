using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic.Controllers;
using System.IO;
using CoffeeProject.Layers;
using MagicDustLibrary.Factorys;

namespace CoffeeProject.GameObjects
{
    [EndlessMove]
    [MoveFreeSpan(0.2)]
    public class BenRunAway : UnitMove<Ben, GameObject>
    {
        public override int GetAttraction(Ben unit, GameObject target)
        {
            return 1;
        }
        public override void OnStart(IControllerProvider state, Ben unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.AddVector("Backward", new MovementVector(
                -Vector2.Normalize(unit.Position - target.GetComponents<IBodyComponent>().First().Position) * 3, 0, TimeSpan.Zero, true));
        }
        public override bool Continue(IControllerProvider state, Ben unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            physics.DirectVector("Backward", unit.Position - targetPosition);
            if (Vector2.Distance(unit.Position, targetPosition) < 500)
            {
                return true;
            }
            return false;
        }

        public override void OnEnd(IControllerProvider state, Ben unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.RemoveVector("Backward");
        }

        public override void OnForcedBreak(IControllerProvider state, Ben unit, GameObject target, UnitMove<Ben, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [EndlessMove]
    [MoveFreeSpan(0.2)]
    public class BenGoForward : UnitMove<Ben, GameObject>
    {
        public override int GetAttraction(Ben unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) > 700)
            {
                return 4;
            }
            return 0;
        }
        public override void OnStart(IControllerProvider state, Ben unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.AddVector("Forward", new MovementVector(
                -Vector2.Normalize(target.GetComponents<IBodyComponent>().First().Position - unit.Position) * 3, 0, TimeSpan.Zero, true));
        }
        public override bool Continue(IControllerProvider state, Ben unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            physics.DirectVector("Forward", targetPosition - unit.Position);
            if (Vector2.Distance(unit.Position, targetPosition) > 700)
            {
                return true;
            }
            return false;
        }

        public override void OnEnd(IControllerProvider state, Ben unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.RemoveVector("Forward");
        }

        public override void OnForcedBreak(IControllerProvider state, Ben unit, GameObject target, UnitMove<Ben, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [MoveDuration(1)]
    [MoveFreeSpan(0.2)]
    [MovePriority(3)]
    [MoveStepsRequired(1)]
    public class BenAttack : UnitMove<Ben, GameObject>
    {
        public override int GetAttraction(Ben unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) <= 500)
            {
                return 0;
            }
            return 2;
        }

        public override bool Continue(IControllerProvider state, Ben unit, GameObject target)
        {
            return true;
        }

        public override void OnStart(IControllerProvider state, Ben unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            unit.Animator.SetAnimation($"Atk_{unit.Animator.Running.Name}", 0);
            state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "enemy_with_fareballs_reload"), "enemy_with_fireballs").Play();
        }

        public override void OnEnd(IControllerProvider state, Ben unit, GameObject target)
        {
            unit.Animator.SetAnimation("Default", 0);
            var direction = target.GetComponents<IBodyComponent>().First().Position - unit.Position;
            direction.Normalize();
            DamageBall.CastBall(
                state, 
                unit.Position + direction * 80, 
                new MovementVector(direction * 8, 0, TimeSpan.FromSeconds(4), false), 
                target.GetComponents<Dummy>().First(),
                unit.Level
                );
        }
    }

    [MoveDuration(1)]
    [MoveFreeSpan(0.2)]
    [MovePriority(3)]
    [MoveStepsRequired(1)]
    public class BenSecondAttack : UnitMove<Ben, GameObject>
    {
        public override int GetAttraction(Ben unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) <= 500)
            {
                return 0;
            }
            return 2;
        }

        public override bool Continue(IControllerProvider state, Ben unit, GameObject target)
        {
            return true;
        }

        public override void OnStart(IControllerProvider state, Ben unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            unit.Animator.SetAnimation($"Atk_{unit.Animator.Running.Name}", 0);
            state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "enemy_with_fareballs_reload"), "enemy_with_fireballs").Play();
        }

        public override void OnEnd(IControllerProvider state, Ben unit, GameObject target)
        {
            unit.Animator.SetAnimation("Default", 0);
            var direction = target.GetComponents<IBodyComponent>().First().Position - unit.Position;
            var direction1 = MathEx.AngleToVector(MathEx.NormalizeAngle(MathEx.VectorToAngle(direction) + 0.1f * MathF.PI, MathF.PI * 2));
            var direction2 = MathEx.AngleToVector(MathEx.NormalizeAngle(MathEx.VectorToAngle(direction) - 0.1f * MathF.PI, MathF.PI * 2));
            DamageBall.CastBall(
                state,
                unit.Position + direction1 * 80,
                new MovementVector(direction1 * 8, 0, TimeSpan.FromSeconds(4), false),
                target.GetComponents<Dummy>().First(), unit.Level
                );
            DamageBall.CastBall(
                state,
                unit.Position + direction2 * 80,
                new MovementVector(direction2 * 8, 0, TimeSpan.FromSeconds(4), false),
                target.GetComponents<Dummy>().First(), unit.Level
                );
            unit.InvokeAttack(state, target);
        }
    }

    [SpriteSheet("enemy_with_fireballs")]
    public class Ben : Sprite, IMultiBehaviorComponent, IUpdateComponent, ICollisionChecker<Hero>, IEnemy
    {
        public Ben(IAnimationProvider provider) : base(provider)
        {
            this.CombineWith(
                new Physics(
                new SurfaceMap([], 0, 1)
                ));
            var unit = new Unit<Ben, GameObject>();
            this.CombineWith(
                new TimerHandler());
            this.CombineWith(
                unit
                );
            unit.AddAction("firstAttack", new BenAttack());
            unit.AddAction("goBackward", new BenRunAway());
            unit.AddAction("secondAttack", new BenSecondAttack());
            unit.AddAction("goForward", new BenGoForward());
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };
        public event Action<IControllerProvider, GameObject> OnAttack = delegate { };
        public void InvokeAttack(IControllerProvider state, GameObject target)
        {
            OnAttack(state, target);
        }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = info.Scale * new Vector2(0.06f, 0.06f);
                info.OrderComparer = Position.ToPoint().Y;
                return info;
            }
        }

        public int Level { get; set; } = 1;

        public void SetTarget(IControllerProvider state, GameObject target)
        {
            this.InvokeEach<Unit<Ben, GameObject>>(it => it.SetTarget(state, target, this));
        }

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);

            var physics = GetComponents<Physics>().First();
            var movement = physics.GetResultingVector(deltaTime);
            movement.Normalize();

            if (movement.Length() is float.NaN)
            {
                return;
            }

            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
            {
                if (movement.X > 0)
                {
                    Animator.SetAnimation("Right", 0);
                }
                else
                {
                    Animator.SetAnimation("Left", 0);
                }
            }
            else
            {
                if (movement.Y > 0)
                {
                    Animator.SetAnimation("Default", 0);
                }
                else
                {
                    Animator.SetAnimation("Backward", 0);
                }
            }

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
                var target = GetComponents<Unit<Ben, GameObject>>().First().Target;
                if (target is null)
                {
                    return;
                }
                memory.Target = target.GetComponents<IBodyComponent>().First();
                memory.Amount = 15 + Level * 5;
                Dispose();
            }
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            var dummy = obj.GetComponents<Dummy>().First();
            var damage = new Dictionary<DamageType, int>
            {
                { DamageType.Physical, 2 + Level }
            };
            var kbvector = obj.Position - this.Position;
            kbvector.Normalize();
            dummy.TakeDamage(new DamageInstance(
                damage,
                Team.enemy,
                ["knockback", $"kbvector={kbvector.X};{kbvector.Y}"],
                "DashAttack",
                GetComponents<Dummy>().First(),
                [], [],
                TimeSpan.FromSeconds(1))
                );
        }
    }
}
