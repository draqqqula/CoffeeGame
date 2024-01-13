using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.Layers;
using CoffeeProject.SurfaceMapping;
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
using SharpDX.MediaFoundation;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("petaly_attack")]
    public class PetalyAttack : Sprite, ICollisionChecker<Hero>
    {
        private DamageInstance Damage { get; set; }
        public PetalyAttack(IAnimationProvider provider) : base(provider)
        {
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            if (Animator.CurrentFrame != 5)
            {
                return;
            }
            var dummyList = obj.GetComponents<Dummy>();
            if (!dummyList.Any())
            {
                return;
            }
            var dummy = dummyList.First();
            if (dummy.Team == Damage.Team)
            {
                return;
            }
            dummy.TakeDamage(Damage);
        }

        public PetalyAttack UseDamage(DamageInstance damage)
        {
            Damage = damage;
            return this;
        }

        public static PetalyAttack CreateAttack(IControllerProvider state, Vector2 position, Dummy owner)
        {
            var damages = new Dictionary<DamageType, int>
            {
                { DamageType.Physical, 3 }
            };

                var obj = state.Using<IFactoryController>()
                .CreateObject<PetalyAttack>()
                .SetPos(position)
                .SetBounds(new Rectangle(-10, -10, 20, 20))
                .SetPlacement(new Placement<MainLayer>())
                .AddToState(state);
            obj.Animator.OnEnded += (name) => obj.Dispose();
            return obj.UseDamage(new DamageInstance(damages, Team.enemy, [], "PetalyAttack", owner, [], [], TimeSpan.FromSeconds(1)));
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { Scale = new Vector2(1.0f, 1.0f), OrderComparer = Position.ToPoint().Y };
    }
}
