using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Elements
{
    public class IceSpawner : NodeComponent
    {
        public IceSpawner()
        {
            AddGreetingFor<IEnemy>((obj) =>
            {
                obj.OnAttack += (state, target) => SpawnProjectile(state, obj, target);
            });
        }

        private const float Speed = 7;
        private void SpawnProjectile(IControllerProvider state, Vector2 position, Vector2 targetPosition)
        {
            var physics = new Physics(state.Using<SurfaceMapProvider>().GetMap("level"));
            var obj = state.Using<IFactoryController>().CreateObject<IceBullet>()
                .SetPos(position)
                .SetPlacement(new Placement<MainLayer>())
                .SetBounds(new Rectangle(-7, -15, 15, 15))
                .AddComponent(physics)
                .AddShadow(state)
                .AddToState(state);
            var direction = targetPosition - position;
            obj.Rotation = MathEx.VectorToAngle(direction);
            direction.Normalize();
            physics.AddVector("shoot", new MovementVector(Speed * direction, 0, TimeSpan.Zero, true));
        }

        private void SpawnProjectile(IControllerProvider state, IEnemy enemy, GameObject target)
        {
            SpawnProjectile(state, ((IBodyComponent)enemy).Position, target.GetComponents<IBodyComponent>().First().Position);
        }
    }

    [SpriteSheet("ice_bullet")]
    public class IceBullet : Sprite, ICollisionChecker<Hero>, IMultiBehaviorComponent
    {
        private readonly DamageInstance _damage;
        private const float Elevation = 70;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public float Rotation { get; set; } = 0;
        public IceBullet(IAnimationProvider provider) : base(provider)
        {
            _damage = new DamageInstance(new Dictionary<DamageType, int>() { { DamageType.Ice, 1 } }, Team.enemy, [], "Fire", null, [], [], TimeSpan.FromSeconds(0.3));
        }

        protected override DrawingParameters DisplayInfo => base.DisplayInfo with { 
            OrderComparer = Position.ToPoint().Y, 
            Position = Position - Vector2.UnitY * Elevation,
            Rotation = Rotation,
            Scale = new Vector2(0.6f, 0.6f)
        };

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

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            obj.InvokeEach<Dummy>(it => it.TakeDamage(_damage));
        }
    }
}
