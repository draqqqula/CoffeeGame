using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Combat;
using CoffeeProject.Family;
using CoffeeProject.Layers;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using RectangleFLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoffeeProject.BoxDisplay;
using System.IO;
using CoffeeProject.Weapons;

namespace CoffeeProject.GameObjects
{
    [Box(100, 200, 50, 200)]
    [MemberShip<TimeStopImmune>]
    [SpriteSheet("hero")]
    public class Hero : Sprite, IMultiBehaviorComponent, IUpdateComponent, IFamilyComponent
    {
        public IPlayerWeapon Weapon { get; set; }
        public IPlayerAbility Ability { get; set; }
        public bool AbilityInputBlocked { get; set; } = false;
        public bool WeaponInputBlocked { get; set; } = false;
        public bool DirectionAnimationForced { get; set; } = false;
        public GameClient Client {  get; set; }

        private List<PlayerModifier> _modifiers = new List<PlayerModifier>();
        public int Currency { get; set; } = 0;
        public Hero(IAnimationProvider provider) : base(provider)
        {
            this.CombineWith(
                new Physics(
                new SurfaceMap([], 0, 1)
                ))
            .CombineWith(
                new Dummy(
                16, [], Team.player, [], [OnDamage], 1
                ))
            .CombineWith(new Spring(0.1f))
            .CombineWith(new TimerHandler());
            Animator.OnEnded += (name) =>
            {
                DirectionAnimationForced = true;
            };
        }

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

        const float SPEED = 4;
        const float DECELERATION = 15;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public event Action OnDamageEvent = delegate { };
        public DamageInstance OnDamage(Dummy dummy, DamageInstance instance)
        {
            OnDamageEvent();
            var spring = GetComponents<Spring>().Last();
            spring.Pull(1.12f);
            if (instance.Tags.Contains("knockback"))
            {
                var physics = GetComponents<Physics>().Last();
                var kbvector = instance.Tags.Where(it => it.StartsWith("kbvector")).First();
                var vector = new Vector2(
                    float.Parse(
                        Regex.Match(kbvector, "(?<==).+(?=;)").Value),
                    float.Parse(
                        Regex.Match(kbvector, "(?<=;).+").Value)
                    );
                physics.AddVector("knockback", new MovementVector(vector * 4, -4, TimeSpan.Zero, true));
            }
            return instance;
        }

        public List<Direction> Directions { get; set; } = [];
        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            Directions.Clear();
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
            var physics = GetComponents<Physics>().Last();
            var dummy = GetComponents<Dummy>().Last();
            var speed = SPEED;
            var deceleration = DECELERATION;

            SetDefaults();
            ApplyModifiers();

            Vector2 resultingVector = Vector2.Zero;
            
            if (Client.Controls[Control.left])
            {
                Directions.Add(Direction.Left);
            }
            if (Client.Controls[Control.right])
            {
                Directions.Add(Direction.Right);
            }
            if (Client.Controls[Control.lookUp])
            {
                Directions.Add(Direction.Backward);
            }
            if (Client.Controls[Control.lookDown])
            {
                Directions.Add(Direction.Forward);
            }

            foreach (var direction in Directions)
            {
                resultingVector += direction.ToPoint().ToVector2();
            }

            if (resultingVector != Vector2.Zero)
            {
                resultingVector.Normalize();
                physics.AddVector("move", new MovementVector(speed * resultingVector, -deceleration, TimeSpan.Zero, true));
            }

            if (Client.Controls.OnPress(Control.pause))
            {
                state.Using<ILevelController>().PauseCurrent();
                state.Using<ILevelController>().LaunchLevel("pause", new LevelArgs(state.Using<ILevelController>().GetCurrentLevelName()), false);
            }

            if (DirectionAnimationForced)
            {
                if (Directions.Count > 0)
                {
                    Animator.SetAnimation(Directions
                        .Last()
                        .ToString()
                        .Replace("Forward", "Default"), 0);
                }

                if (physics.ActiveVectors.Where(it => it.Key.StartsWith("move")).Any())
                {
                    Animator.Resume();
                }
                else
                {
                    Animator.SetFrame(0);
                    Animator.Stop();
                }
            }

            if (!dummy.IsAlive)
            {
                state.Using<ILevelController>().PauseCurrent();
                state.Using<ILevelController>().LaunchLevel("gameover", false);
            }

            ManageWeapon(state);
            ManageAbility(state);
        }

        public Hero UseWeapon(IPlayerWeapon weapon)
        {
            Weapon = weapon;
            return this;
        }

        public Hero UseAbility(IPlayerAbility ability)
        {
            Ability = ability;
            return this;
        }

        private void ManageWeapon(IControllerProvider state)
        {
            if (Weapon is null || WeaponInputBlocked)
            {
                return;
            }

            if (Client.Controls.OnPress(Control.jump))
            {
                Weapon.UsePrimary(state, this);
            }
        }

        private void ManageAbility(IControllerProvider state)
        {
            if (Ability is null || AbilityInputBlocked)
            {
                return;
            }

            if (Client.Controls.OnPress(Control.dash))
            {
                Ability.UseAbility(state, this);
            }
        }

        private void ApplyModifiers()
        {
            foreach (var modifier in _modifiers.Where(it => it.HasUpdate))
            {
                modifier.ApplyOnUpdate(this);
            }
        }

        private void SetDefaults()
        {
            foreach (var container in GetComponents<IModifiableContainer>())
            {
                container.SetDefaults();
            }
        }

        public void AddModifier(PlayerModifier modifier)
        {
            _modifiers.Add(modifier);
            modifier.ApplyOnce(this);
        }

        public void RemoveModifier(PlayerModifier modifier)
        {
            modifier.Drop(this);
            _modifiers.Remove(modifier);
        }
    }

    public static class ControlsExtensions
    {
        public static Vector2 ToVector(this Control control)
        {
            switch (control)
            {
                case Control.left:
                    return new Vector2(-1, 0);
                case Control.right:
                    return new Vector2(1, 0);
                case Control.lookUp:
                    return new Vector2(0, -1);
                case Control.lookDown:
                    return new Vector2(0, 1);
                default:
                    return Vector2.Zero;
            }    
        }
    }


    public class MyList : List<PlayerModifier>
    {
        public void Add(PlayerModifier modifier)
        {
            base.Add(modifier);
        }
    }
}
