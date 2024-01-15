﻿using BehaviorKit;
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
    [SpriteSheet("damage_ball")]
    internal class DamageBall : Sprite, IUpdateComponent, IMultiBehaviorComponent, ICollisionChecker<Hero>
    {
        private DamageInstance Damage { get; set; }
        public DamageBall(IAnimationProvider provider) : base(provider)
        {
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
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
                Collapse(state);
            }
        }

        public DamageBall UseDamage(DamageInstance damage)
        {
            Damage = damage;
            return this;
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { Scale = new Vector2(0.6f, 0.6f), OrderComparer = Position.ToPoint().Y };

        public static DamageBall CastBall(IControllerProvider state, Vector2 position, MovementVector vector, Dummy owner, int level)
        {
            var map = state.Using<SurfaceMapProvider>().GetMap("level");
            var damages = new Dictionary<DamageType, int>
            {
                { DamageType.Fire, 4 + level }
            };
            var physics = new Physics(map);
            var timerHandler = new TimerHandler();
            physics.AddVector("move", vector);
            var obj = state.Using<IFactoryController>()
                .CreateObject<DamageBall>()
                .SetPos(position)
                .SetBounds(new Rectangle(-15, -15, 30, 30))
                .SetPlacement(new Placement<MainLayer>())
                .AddShadow(state)
                .AddComponent(physics)
                .AddComponent(timerHandler)
                .AddToState(state);
            timerHandler.SetTimer("dispose", 4, obj.Dispose, true);
            return obj.UseDamage(new DamageInstance(damages, Team.enemy, [], "DamageBall", owner, [], [(target, dmg) =>
            {
                obj.Collapse(state);
                return dmg;
            }
            ], TimeSpan.FromSeconds(1)));
        }

        public void Collapse(IControllerProvider state)
        {
            Dispose();
            state.Using<IFactoryController>()
                .CreateObject<DamageBallCollapse>()
                .SetPlacement(new Placement<MainLayer>())
                .SetPos(Position)
                .AddToState(state);
        }
    }

    [SpriteSheet("collapse")]
    public class DamageBallCollapse : Sprite
    {
        public DamageBallCollapse(IAnimationProvider provider) : base(provider)
        {
            Animator.OnEnded += (name) => Dispose();
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { Scale = new Vector2(0.6f, 0.6f), OrderComparer = Position.ToPoint().Y };
    }
}
