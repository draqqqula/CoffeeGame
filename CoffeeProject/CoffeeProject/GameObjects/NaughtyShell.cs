using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.ComponentModel;
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
    [EndlessMove]
    [MoveFreeSpan(0.2)]
    public class EnemyGoForward : UnitMove<NaughtyShell, GameObject>
    {
        public override int GetAttraction(NaughtyShell unit, GameObject target)
        {
            return 1;
        }
        public override void OnStart(IControllerProvider state, NaughtyShell unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.AddVector("Forward", new MovementVector(
                -Vector2.Normalize(unit.Position - target.GetComponents<IBodyComponent>().First().Position) * 5, 0, TimeSpan.Zero, true));
        }
        public override bool Continue(IControllerProvider state, NaughtyShell unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            physics.DirectVector("Forward", targetPosition - unit.Position);
            if (Vector2.Distance(unit.Position, targetPosition) > 300)
            {
                return true;
            }
            return false;
        }

        public override void OnEnd(IControllerProvider state, NaughtyShell unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            physics.RemoveVector("Forward");
        }

        public override void OnForcedBreak(IControllerProvider state, NaughtyShell unit, GameObject target, UnitMove<NaughtyShell, GameObject> breaker)
        {
            OnEnd(state, unit, target);
        }
    }

    [EndlessMove]
    [MoveFreeSpan(0.2)]
    [MovePriority(3)]
    public class DashAttack : UnitMove<NaughtyShell, GameObject>
    {
        public override int GetAttraction(NaughtyShell unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            if (Vector2.Distance(unit.Position, targetPosition) > 300)
            {
                return 0;
            }
            return 2;
        }

        public override bool Continue(IControllerProvider state, NaughtyShell unit, GameObject target)
        {
            var physics = unit.GetComponents<Physics>().First();
            return physics.ActiveVectors.ContainsKey("DashAttack");
        }

        public override void OnStart(IControllerProvider state, NaughtyShell unit, GameObject target)
        {
            var targetPosition = target.GetComponents<IBodyComponent>().First().Position;
            var physics = unit.GetComponents<Physics>().First();
            physics.AddVector("DashAttack", new MovementVector(Vector2.Normalize(unit.Position - targetPosition) * -7, -5, TimeSpan.Zero, true));
        }
    }

    [SpriteSheet("enemy")]
    public class NaughtyShell : Sprite, IMultiBehaviorComponent, IUpdateComponent, ICollisionChecker<Hero>, IEnemy
    {
        public NaughtyShell(IAnimationProvider provider) : base(provider)
        {
            this.CombineWith(
                new Physics(
                new SurfaceMap([], 0, 1)
                ));
            this.CombineWith(
                new Dummy(
                16, [], Team.enemy, [], [], 1
                ));
            var unit = new Unit<NaughtyShell, GameObject>();
            this.CombineWith(
                new TimerHandler());
            this.CombineWith(
                unit
                );
            unit.AddAction("dashAttack", new DashAttack());
            unit.AddAction("goForward", new EnemyGoForward());
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = info.Scale * new Vector2(0.05f, 0.05f);
                info.OrderComparer = Position.ToPoint().Y;
                return info;
            }
        }

        public void SetTarget(IControllerProvider state, GameObject target)
        {
            this.InvokeEach<Unit<NaughtyShell, GameObject>>(it => it.SetTarget(state, target, this));
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
                Dispose();
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
                [ "knockback", $"kbvector={kbvector.X};{kbvector.Y}" ], 
                "DashAttack", 
                GetComponents<Dummy>().First(), 
                [], [], 
                TimeSpan.FromSeconds(1))
                );
        }
    }
}
