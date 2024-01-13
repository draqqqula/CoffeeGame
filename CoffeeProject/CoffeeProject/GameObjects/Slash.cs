using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.Family;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("slash")]
    [MemberShip<TimeStopImmune>]
    public class Slash : Sprite, IUpdateComponent, ICollisionChecker<IBodyComponent>, IFamilyComponent
    {
        public DamageInstance Damage { get; set; }
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Hero Owner { get; set; }
        public Slash(IAnimationProvider provider) : base(provider)
        {
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { 
            OrderComparer = Position.ToPoint().Y, 
            Scale = new Microsoft.Xna.Framework.Vector2(0.6f, 0.6f) * (Bounds.Size.ToVector2() / 140),
        };

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            if (Owner is not null)
            {
                this.SetPos(Owner.Position + Offset);
            }
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, IBodyComponent obj, Rectangle intersection)
        {
            var ownerDummy = Owner.GetComponents<Dummy>().First();
            if (obj is GameObject gobj)
            {
                var dummies = gobj.GetComponents<Dummy>();
                if (dummies.Any())
                {
                    var dummy = dummies.First();
                    if (dummy.Team == ownerDummy.Team)
                    {
                        return;
                    }
                    Damage.Events.Clear();
                    Damage.Events.Add((target, dmg) =>
                    {
                        DealKnockBack(obj);
                        return dmg;
                    });
                    dummy.TakeDamage(Damage);
                }
            }
        }

        private const float KnockBackPower = 6;
        private const float KnockBackAcceleration = -1;
        private const double KnockBackLifetime = 0.3;
        private void DealKnockBack(IBodyComponent obj)
        {
            if (obj is GameObject gobj)
            {
                var physicsList = gobj.GetComponents<Physics>();
                if (!physicsList.Any())
                {
                    return;
                }
                var initialKnockback = Offset;
                initialKnockback.Normalize();
                physicsList.First().AddVector("knockback",
                    new MovementVector(initialKnockback * KnockBackPower,
                    KnockBackAcceleration,
                    TimeSpan.FromSeconds(KnockBackLifetime),
                    false)
                    );
            }
        }
    }
}
