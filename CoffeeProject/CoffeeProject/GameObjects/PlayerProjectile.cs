using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.Layers;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("player_projectile")]
    internal class PlayerProjectile : Sprite, IUpdateComponent, IMultiBehaviorComponent, ICollisionChecker<IBodyComponent>
    {
        private DamageInstance Damage { get; set; }
        public PlayerProjectile(IAnimationProvider provider) : base(provider)
        {
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, IBodyComponent obj, Rectangle intersection)
        {
            if (obj is GameObject gobj)
            {
                var dummyList = gobj.GetComponents<Dummy>();
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
        }

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);

            var physics = GetComponents<Physics>().First();
            if (physics.Faces[Side.Top] ||
                physics.Faces[Side.Left] ||
                physics.Faces[Side.Right] ||
                physics.Faces[Side.Bottom])
            {
                Dispose();
            }
        }

        public PlayerProjectile UseDamage(DamageInstance damage)
        {
            Damage = damage;
            return this;
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { Scale = new Vector2(0.6f, 0.6f), OrderComparer = Position.ToPoint().Y };

        private const float Speed = 10;
        public static PlayerProjectile ShootProjectile(IControllerProvider state, Vector2 position, Direction vector, Dummy owner, string weapon)
        {
            var map = state.Using<SurfaceMapProvider>().GetMap("level");
            var damages = new Dictionary<DamageType, int>
            {
                { DamageType.Physical, 5 }
            };
            var physics = new Physics(map);
            var timerHandler = new TimerHandler();
            physics.AddVector("move", new MovementVector(Speed * vector.ToPoint().ToVector2(), 0, TimeSpan.FromSeconds(4), false));
            var obj = state.Using<IFactoryController>()
                .CreateObject<PlayerProjectile>()
                .SetPos(position + Speed * vector.ToPoint().ToVector2() * 0.1f)
                .SetBounds(new Rectangle(-15, -30, 30, 30))
                .SetPlacement(new Placement<MainLayer>())
                .AddShadow(state)
                .AddComponent(physics)
                .AddComponent(timerHandler)
                .AddToState(state);
            obj.Animator.SetAnimation((vector.ToString() + "_" + weapon).Replace("Forward_Arrow", "Default"), 0);
            timerHandler.SetTimer("dispose", 4, obj.Dispose, true);
            return obj.UseDamage(new DamageInstance(damages, Team.player, [], "DamageBall", owner, [], [(target, dmg) =>
            {
                obj.Dispose();
                return dmg;
            }
            ], TimeSpan.FromSeconds(1)));
        }
    }
}
