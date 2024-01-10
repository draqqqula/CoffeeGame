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
using MagicDustLibrary.Animations;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Extensions;
using CoffeeProject.BoxDisplay;
using MagicDustLibrary.Logic.Controllers;
using CoffeeProject.Layers;

namespace CoffeeProject.GameObjects
{
    [MoveDuration(1)]
    [MoveFreeSpan(0)]
    [MovePriority(1)]
    [MoveCooldown(0)]
    public class DemonGoForward : UnitMove<Demon, GameObject>
    {
        public override int GetAttraction(Demon unit, GameObject target)
        {
            return 1;
        }
        public override void OnStart(IControllerProvider state, Demon unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.AddVector("Forward", new MovementVector(
                -Vector2.Normalize(unit.Position - target.GetComponents<IBodyComponent>().First().Position) * 3, 0, TimeSpan.Zero, true));
        }
        public override bool Continue(IControllerProvider state, Demon unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (!physics.Vectors.ContainsKey("Forward"))
            {
                physics.AddVector("Forward", new MovementVector(
                -Vector2.Normalize(unit.Position - target.GetComponents<IBodyComponent>().First().Position) * 3, 0, TimeSpan.Zero, true));
                physics.DirectVector("Forward", targetPosition - unit.Position);
            }

            physics.DirectVector("Forward", targetPosition - unit.Position);
            var movement = physics.Vectors["Forward"].Vector;

            ApplyAnimation(movement, unit);

            if (Vector2.Distance(unit.Position, targetPosition) > 300)
            {
                return true;
            }
            return false;
        }

        private bool OnShakeFrame { get; set; } = false;
        private void ApplyAnimation(Vector2 movement, Demon unit)
        {
            if (movement.Length() is float.NaN)
            {
                return;
            }

            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
            {
                if (movement.X > 0)
                {
                    unit.Animator.SetAnimation("Right", 0);
                }
                else
                {
                    unit.Animator.SetAnimation("Left", 0);
                }
            }
            else
            {
                if (movement.Y > 0)
                {
                    unit.Animator.SetAnimation("Default", 0);
                }
                else
                {
                    unit.Animator.SetAnimation("Backward", 0);
                }
            }
            if ((unit.Animator.Running.CurrentFrame == 1 ||
                unit.Animator.Running.CurrentFrame == 3)
                && !OnShakeFrame)
            {
                OnShakeFrame = true;
                unit.InvokeEach<VisualShake>(it => it.Start(15, TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.025), 4, 2));
            }
        }

        public override void OnEnd(IControllerProvider state, Demon unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.RemoveVector("Forward");
        }

        public override void OnForcedBreak(IControllerProvider state, Demon unit, GameObject target, UnitMove<Demon, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [EndlessMove]
    [MoveFreeSpan(0.2)]
    [MovePriority(3)]
    public class DemonAttack : UnitMove<Demon, GameObject>
    {
        private DamageBox DamageBox { get; set; }
        private Side Side { get; set; }
        public override int GetAttraction(Demon unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) > 300)
            {
                return 0;
            }
            return 2;
        }

        public override bool Continue(IControllerProvider state, Demon unit, GameObject target)
        {
            DamageBox.SetPos(unit.Position);
            MoveDamageBox(unit);
            if (unit.Animator.Running.CurrentFrame == 2)
            {
                DamageBox.Active = true;
            }
            else
            {
                DamageBox.Active = false;
            }

            return unit.Animator.Running.Name.StartsWith("Atk");
        }

        const float BoxDistance = 80;
        private void MoveDamageBox(Demon unit)
        {
            DamageBox.SetPos(unit.Position + Side.ToPoint().ToVector2() * BoxDistance);
        }

        private DamageInstance GetDamage(Vector2 knockback, Dummy owner)
        {
            knockback.Normalize();
            var damage = new Dictionary<DamageType, int>
                {
                    { DamageType.Physical, 3 }
                };
            return new DamageInstance(
                damage,
                Team.enemy,
                ["knockback", $"kbvector={knockback.X};{knockback.Y}"],
                "DashAttack",
                owner,
                [], [],
                TimeSpan.FromSeconds(1));
        }

        public override void OnStart(IControllerProvider state, Demon unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            var deltaPosition = targetPosition - unit.Position;
            var physics = unit.GetComponents<Physics>().First();

            if (Math.Abs(deltaPosition.X) > Math.Abs(deltaPosition.Y))
            {
                if (deltaPosition.X > 0)
                {
                    unit.Animator.SetAnimation("Atk_right", 0);
                    physics.AddVector("Attack", new MovementVector(new Vector2(1, 0) * 5, -1, TimeSpan.Zero, true));
                    Side = Side.Right;
                }
                else
                {
                    unit.Animator.SetAnimation("Atk_left", 0);
                    physics.AddVector("Attack", new MovementVector(new Vector2(-1, 0) * 5, -1, TimeSpan.Zero, true));
                    Side = Side.Left;
                }
            }
            else
            {
                if (deltaPosition.Y > 0)
                {
                    unit.Animator.SetAnimation("Atk_forward", 0);
                    physics.AddVector("Attack", new MovementVector(new Vector2(0, 1) * 5, -1, TimeSpan.Zero, true));
                    Side = Side.Bottom;
                }
                else
                {
                    unit.Animator.SetAnimation("Atk_backward", 0);
                    physics.AddVector("Attack", new MovementVector(new Vector2(0, -1) * 5, -1, TimeSpan.Zero, true));
                    Side = Side.Top;
                }
            }

            DamageBox = new DamageBox(GetDamage(Side.ToPoint().ToVector2(), unit.GetComponents<Dummy>().First()))
                .SetPos(unit.Position)
                .SetBounds(new Rectangle(-30, -30, 60, 60))
                .UseBoxDisplay(state, Color.Green, Color.Purple, 3)
                .AddToState(state);
        }

        public override void OnEnd(IControllerProvider state, Demon unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            DamageBox.Dispose();
            DamageBox = null;
            physics.RemoveVector("Attack");
        }

        public override void OnForcedBreak(IControllerProvider state, Demon unit, GameObject target, UnitMove<Demon, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }


    [MoveDuration(0.4)]
    [MoveFreeSpan(0.1)]
    [MovePriority(3)]
    [MoveStepsRequired(3)]
    public class DemonDashBackward : UnitMove<Demon, GameObject>
    {
        public override int GetAttraction(Demon unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) > 200)
            {
                return 0;
            }
            return 3;
        }

        public override bool Continue(IControllerProvider state, Demon unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            return physics.Vectors.ContainsKey("Backward");
        }

        public override void OnStart(IControllerProvider state, Demon unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            var deltaPosition = targetPosition - unit.Position;
            var physics = unit.GetComponents<Physics>().First();
            physics.AddVector("Backward", new MovementVector(
                -Vector2.Normalize(target.GetComponents<IBodyComponent>().First().Position - unit.Position) * 4, -2, TimeSpan.Zero, true));
        }

        public override void OnEnd(IControllerProvider state, Demon unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.RemoveVector("Backward");
        }

        public override void OnForcedBreak(IControllerProvider state, Demon unit, GameObject target, UnitMove<Demon, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [EndlessMove]
    [MoveFreeSpan(0.9)]
    [MovePriority(3)]
    [MoveCooldown(10)]
    public class DemonTeleport : UnitMove<Demon, GameObject>
    {
        public override int GetAttraction(Demon unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) > 0)
            {
                return 5;
            }
            return 0;
        }

        public override bool Continue(IControllerProvider state, Demon unit, GameObject target)
        {
            return unit.Animator.Running.Name.StartsWith("Teleport");
        }

        public override void OnStart(IControllerProvider state, Demon unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            var physics = unit.GetComponents<Physics>().First();
            unit.Animator.SetAnimation("Teleport_start", 0);
            TeleportObject = state.Using<IFactoryController>().CreateObject<BlackTeleport>().SetPos(targetPosition)
                .SetPlacement(new Placement<MainLayer>()).AddToState(state);
            LandingPosition = targetPosition;
        }

        public override void OnEnd(IControllerProvider state, Demon unit, GameObject target)
        {
            unit.SetPos(LandingPosition);
            unit.Animator.SetAnimation("Teleport_end", 0);
            var direction1 = MathEx.AngleToVector(MathF.PI / 4);
            var direction2 = MathEx.AngleToVector(MathF.PI / 4 + MathF.PI * 0.5f);
            var direction3 = MathEx.AngleToVector(MathF.PI / 4 + MathF.PI);
            var direction4 = MathEx.AngleToVector(MathF.PI / 4 + MathF.PI * 1.5f);
            TeleportObject.Dispose();
            DamageBall.CastBall(
                state,
                unit.Position + direction1 * 80,
                new MovementVector(direction1 * 8, 0, TimeSpan.FromSeconds(4), false),
                target.GetComponents<Dummy>().First()
                );
            DamageBall.CastBall(
                state,
                unit.Position + direction2 * 80,
                new MovementVector(direction2 * 8, 0, TimeSpan.FromSeconds(4), false),
                target.GetComponents<Dummy>().First()
                );
            DamageBall.CastBall(
                state,
                unit.Position + direction3 * 80,
                new MovementVector(direction3 * 8, 0, TimeSpan.FromSeconds(4), false),
                target.GetComponents<Dummy>().First()
                );
            DamageBall.CastBall(
                state,
                unit.Position + direction4 * 80,
                new MovementVector(direction4 * 8, 0, TimeSpan.FromSeconds(4), false),
                target.GetComponents<Dummy>().First()
                );
        }

        public override void OnForcedBreak(IControllerProvider state, Demon unit, GameObject target, UnitMove<Demon, GameObject> breaker)
        {
            unit.Animator.SetAnimation("Default", 0);
            TeleportObject.Dispose();
        }

        private Vector2 LandingPosition { get; set; } = Vector2.Zero;
        private BlackTeleport TeleportObject { get; set; }
    }

    [EndlessMove]
    [MoveFreeSpan(0.1)]
    [MovePriority(3)]
    public class DemonFallDown : UnitMove<Demon, GameObject>
    {
        public override int GetAttraction(Demon unit, GameObject target)
        {
            var dummy = unit.GetComponents<Dummy>().First();
            if (dummy.IsAlive)
            {
                return 0;
            }
            return 10;
        }

        public override bool Continue(IControllerProvider state, Demon unit, GameObject target)
        {
            return true;
        }

        public override void OnStart(IControllerProvider state, Demon unit, GameObject target)
        {
            var shake = unit.GetComponents<VisualShake>().First();
            unit.GetComponents<TimerHandler>().First().SetTimer("dispose", 3, () => 
            {
                unit.Dispose();
                var memory = state.Using<IFactoryController>()
                .CreateObject<MemoryGate>()
                .SetPlacement(new Placement<MainLayer>())
                .SetPos(unit.Position)
                .AddShadow(state)
                .SetBounds(new Rectangle(-20, -20, 40, 40))
                .AddToState(state);
                if (target is null)
                {
                    return;
                }
                memory.Target = target.GetComponents<IBodyComponent>().First();
            }, true);
            shake.Start(30, TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.05), 7, 3);
        }

        public override void OnEnd(IControllerProvider state, Demon unit, GameObject target)
        {
        }
    }

    [SpriteSheet("boss1")]
    public class Demon : Sprite, IMultiBehaviorComponent, IUpdateComponent, ICollisionChecker<Hero>, IEnemy
    {
        public Demon(IAnimationProvider provider) : base(provider)
        {
            this.CombineWith(
                new Physics(
                new SurfaceMap([], 0, 1)
                ));
            this.CombineWith(
                new Dummy(
                16, [], Team.enemy, [], [], 1
                ));
            var unit = new Unit<Demon, GameObject>();
            this.CombineWith(
                new TimerHandler());
            this.CombineWith(
                unit
                );
            CombineWith(
                new VisualShake()
            );
            unit.AddAction("attack", new DemonGoForward());
            unit.AddAction("goForward", new DemonAttack());
            unit.AddAction("teleport", new DemonTeleport(), 5);
            unit.AddAction("dashBackward", new DemonDashBackward());
            unit.AddAction("fallDown", new DemonFallDown());
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = info.Scale * new Vector2(0.2f, 0.2f);
                info.OrderComparer = Position.ToPoint().Y;
                return info;
            }
        }

        public void SetTarget(IControllerProvider state, GameObject target)
        {
            this.InvokeEach<Unit<Demon, GameObject>>(it => it.SetTarget(state, target, this));
        }

        private bool Dead { get; set; } = false;
        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
            if (!GetComponents<Dummy>().First().IsAlive && !Dead)
            {
                var unit = GetComponents<Unit<Demon, GameObject>>().First();
                unit.ReactWith(state, "fallDown", this);
                Dead = true;
                Animator.SetAnimation("Fallen", 0);
            }
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            var dummy = obj.GetComponents<Dummy>().First();
            var damage = new Dictionary<DamageType, int>
            {
                { DamageType.Physical, 3 }
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
