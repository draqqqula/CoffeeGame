using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.Family;
using CoffeeProject.Layers;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization.DefualtImplementations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("damage_ball")]
    internal class PlayerDamageBall : Sprite, IUpdateComponent, IMultiBehaviorComponent, ICollisionChecker<IBodyComponent>
    {
        public bool Attached { get; set; } = true;
        public IBodyComponent Source { get; set; }
        public float Angle { get; set; } = 0;
        public float RotationSpeed { get; set; } = MathF.PI;
        public float RotatationRadius { get; set; } = 100;
        private DamageInstance Damage { get; set; }
        public PlayerDamageBall(IAnimationProvider provider) : base(provider)
        {
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, IBodyComponent obj, Rectangle intersection)
        {
            if (obj is GameObject gobj && !Attached)
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
                Collapse(state);
            }
        }

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);

            if (!Attached)
            {
                var physics = GetComponents<Physics>().First();
                if (physics.Faces[Side.Top] ||
                    physics.Faces[Side.Left] ||
                    physics.Faces[Side.Right] ||
                    physics.Faces[Side.Bottom])
                {
                    Collapse(state);
                }
                return;
            }

            if (Source is null)
            {
                return;
            }

            Angle = MathEx.NormalizeAngle(Angle + RotationSpeed * (float)deltaTime.TotalSeconds, MathF.PI * 2);
            Position = Source.Position + MathEx.AngleToVector(Angle) * RotatationRadius;
        }

        public PlayerDamageBall UseDamage(DamageInstance damage)
        {
            Damage = damage;
            return this;
        }

        public PlayerDamageBall UseSource(IBodyComponent source)
        {
            Source = source;
            return this;
        }

        private const float Speed = 5f;
        public void Launch(IControllerProvider state, Vector2 position)
        {
            Attached = false;
            var map = state.Using<SurfaceMapProvider>().GetMap("level");
            var physics = new Physics(map);
            this.AddComponent(physics);
            var direction = position - Position;
            direction.Normalize();
            physics.AddVector("launch", new MovementVector(direction * Speed, 0, TimeSpan.Zero, true));
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { 
            Scale = new Vector2(0.6f, 0.6f), 
            OrderComparer = Position.ToPoint().Y, 
            Color = Color.Yellow };

        public static PlayerDamageBall CastBall(IControllerProvider state, Vector2 position, Hero owner, int index)
        {
            var damages = new Dictionary<DamageType, int>
            {
                { DamageType.Holy, 1 }
            };
            var timerHandler = new TimerHandler();
            var obj = state.Using<IFactoryController>()
                .CreateObject<PlayerDamageBall>()
                .SetPos(position)
                .SetBounds(new Rectangle(-15, -15, 30, 30))
                .SetPlacement(new Placement<MainLayer>())
                .AddShadow(state)
                .UseSource(owner)
                .AddComponent(timerHandler)
                .AddToState(state);
            var family = state.Using<IFamilyController>().GetFamily<Enemy>();
            var launchAction = () =>
            {
                if (family.Any())
                {
                    obj.Launch(state, family.MinBy(it => Vector2.Distance(it.Position, obj.Position)).Position);
                }
            };
            owner.OnDamageEvent += launchAction;
            obj.OnDisposeEvent -= (h) => owner.OnDamageEvent -= launchAction;
            launchAction += () => owner.OnDamageEvent -= launchAction;
            timerHandler.SetTimer("dispose", 10, obj.Dispose, true);
            var ownerDummy = owner.GetComponents<Dummy>().First();
            return obj.UseDamage(new DamageInstance(damages, Team.player, [], $"DamageBall{index}", ownerDummy, [], [(dum, dmg) => { ownerDummy.RecieveHealing(1); return dmg; } ], TimeSpan.FromSeconds(1)));
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
}
