using CoffeeProject.Collision;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    public class RoomTrigger : GameObject, IBodyComponent, ICollisionChecker<Hero>
    {
        private bool Activated { get; set; } = true;
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }

        public event Action<Hero> PlayerDetected = delegate { };

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            if (!Activated)
            {
                return;
            }
            PlayerDetected(obj);
            Activated = false;
        }
    }
}
