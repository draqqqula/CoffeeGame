using CoffeeProject.Collision;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    public class SomeTrigger : GameObject, ICollisionChecker<Hero>
    {
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }

        public event OnDispose OnDisposeEvent;

        public void Dispose()
        {
            OnDisposeEvent(this);
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            obj.Position += new Vector2(1, 1);
        }
    }
}
