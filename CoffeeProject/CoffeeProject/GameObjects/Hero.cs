using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Combat;
using CoffeeProject.Family;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using RectangleFLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [Box(100, 200, 50, 200)]
    [SpriteSheet("hero")]
    public class Hero : Sprite, IMultiBehaviorComponent, IUpdateComponent
    {
        public GameClient Client;
        private List<PlayerModifier> _modifiers = new List<PlayerModifier>();
        public Hero(IAnimationProvider provider) : base(provider)
        {
            this.CombineWith(
                new Physics<Hero>(
                new SurfaceMap([], 0, 1)
                ));
            this.CombineWith(
                new Dummy(
                16, [], Team.player, [], [ OnDamage ], 1
                ));
            this.CombineWith(new Spring(0.1f));
        }

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

        const float SPEED = 4;
        const float DECELERATION = 15;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public DamageInstance OnDamage(Dummy dummy, DamageInstance instance)
        {
            var spring = GetComponents<Spring>().Last();
            spring.Pull(1.12f);
            return instance;
        }

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
            var physics = GetComponents<Physics<Hero>>().Last();
            var dummy = GetComponents<Dummy>().Last();
            var speed = SPEED;
            var deceleration = DECELERATION;

            SetDefaults();
            ApplyModifiers();

            Vector2 resultingVector = Vector2.Zero;
            bool refreshAnimator = true;
            
            if (Client.Controls[Control.left])
            {
                if (refreshAnimator)
                {
                    Animator.SetAnimation("Left", 0);
                    refreshAnimator = false;
                }
                resultingVector += new Vector2(-1, 0);
            }
            if (Client.Controls[Control.right])
            {
                if (refreshAnimator)
                {
                    Animator.SetAnimation("Right", 0);
                    refreshAnimator = false;
                }
                resultingVector += new Vector2(1, 0);
            }
            if (Client.Controls[Control.lookUp])
            {
                if (refreshAnimator)
                {
                    Animator.SetAnimation("Backward", 0);
                    refreshAnimator = false;
                }
                resultingVector += new Vector2(0, -1);
            }
            if (Client.Controls[Control.lookDown])
            {
                if (refreshAnimator)
                {
                    Animator.SetAnimation("Default", 0);
                    refreshAnimator = false;
                }
                resultingVector += new Vector2(0, 1);
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

            if (physics.ActiveVectors.Where(it => it.Key.StartsWith("move")).Any())
            {
                Animator.Resume();
            }
            else
            {
                Animator.SetFrame(DisplayInfo ,0);
                Animator.Stop();
            }

            if (!dummy.IsAlive)
            {
                state.Using<ILevelController>().PauseCurrent();
                state.Using<ILevelController>().LaunchLevel("gameover", false);
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
