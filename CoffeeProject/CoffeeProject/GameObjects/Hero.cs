using CoffeeProject.Behaviors;
using CoffeeProject.Combat;
using CoffeeProject.Family;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [Box(10)]
    [SpriteSheet("hero.aseprite")]
    public class Hero : Sprite
    {
        public GameClient Client;
        private List<PlayerModifier> _modifiers = new List<PlayerModifier>();
        public Hero(IPlacement placement, Vector2 position, IAnimationProvider provider) : base(placement, position, provider)
        {
            this.AddBehavior("physics", new Physics(new List<Rectangle[]>().ToArray(), 12, true));
        }

        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = new Vector2(0.5f, 0.5f);
                return info;
            }
        }

        public override void OnTick(IStateController state, TimeSpan deltaTime)
        {
            var physics = GetBehavior<Physics>("physics");
            var speed = 120f * (float)deltaTime.TotalSeconds;

            SetDefaults();
            ApplyModifiers();

            if (Client.Controls[Control.left])
            {
                physics.AddVector("left", new MovementVector(new Vector2(-speed, 0), -5, TimeSpan.Zero, true));
            }
            if (Client.Controls[Control.right])
            {
                physics.AddVector("right", new MovementVector(new Vector2(speed, 0), -5, TimeSpan.Zero, true));
            }
            if (Client.Controls[Control.lookUp])
            {
                physics.AddVector("down", new MovementVector(new Vector2(0, -speed), -5, TimeSpan.Zero, true));
            }
            if (Client.Controls[Control.lookDown])
            {
                physics.AddVector("up", new MovementVector(new Vector2(0, speed), -5, TimeSpan.Zero, true));
            }

            if (Client.Controls.OnPress(Control.pause))
            {
                state.PauseCurrent();
                state.LaunchLevel("pause", new LevelArgs(state.GetCurrentLevelName()), false);
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
