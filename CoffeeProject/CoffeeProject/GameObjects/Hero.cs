using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Combat;
using CoffeeProject.Family;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [Box(100, 200, 50, 200)]
    [SpriteSheet("hero")]
    public class Hero : Sprite
    {
        public GameClient Client;
        private List<PlayerModifier> _modifiers = new List<PlayerModifier>();
        public Hero(IAnimationProvider provider) : base(provider)
        {
            this.AddBehavior("physics", new Physics(new List<Rectangle[]>().ToArray(), 12, true));
            AddBehavior("spring", new Spring(0.1f));
        }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = info.Scale * new Vector2(0.05f, 0.05f);
                return info;
            }
        }

        const float SPEED = 8;
        const float DECELERATION = 15;
        public override void OnTick(IControllerProvider state, TimeSpan deltaTime)
        {
            var physics = GetBehavior<Physics>("physics");
            var spring = GetBehavior<Spring>("spring");
            var speed = 60f*SPEED * (float)deltaTime.TotalSeconds;
            var deceleration = DECELERATION;

            SetDefaults();
            ApplyModifiers();

            if (Client.Controls[Control.left])
            {
                Animator.SetAnimation("Left", 0);
                physics.AddVector("move_left", new MovementVector(new Vector2(-speed, 0), -deceleration, TimeSpan.Zero, true));
            }
            if (Client.Controls[Control.right])
            {
                Animator.SetAnimation("Right", 0);
                physics.AddVector("move_right", new MovementVector(new Vector2(speed, 0), -deceleration, TimeSpan.Zero, true));
            }
            if (Client.Controls[Control.lookUp])
            {
                Animator.SetAnimation("Backward", 0);
                physics.AddVector("move_down", new MovementVector(new Vector2(0, -speed), -deceleration, TimeSpan.Zero, true));
            }
            if (Client.Controls[Control.lookDown])
            {
                Animator.SetAnimation("Default", 0);
                physics.AddVector("move_up", new MovementVector(new Vector2(0, speed), -deceleration, TimeSpan.Zero, true));
            }

            if (Client.Controls.OnPress(Control.left) || Client.Controls.OnPress(Control.right)
                || Client.Controls.OnPress(Control.lookUp) || Client.Controls.OnPress(Control.lookDown))
            {
                spring.Pull(1.12f);
            }

            if (Client.Controls.OnPress(Control.pause))
            {
                state.Using<ILevelController>().PauseCurrent();
                state.Using<ILevelController>().LaunchLevel("pause", new LevelArgs(state.Using<ILevelController>().GetCurrentLevelName()), false);
            }

            if (physics.ActiveVectors.Where(it => it.Key.StartsWith("move_")).Any())
            {
                Animator.Resume();
            }
            else
            {
                Animator.SetFrame(DisplayInfo ,0);
                Animator.Stop();
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
            foreach (var behavior in Behaviors.Values)
            {
                if (behavior is IModifiableContainer container)
                {
                    container.SetDefaults();
                }
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
