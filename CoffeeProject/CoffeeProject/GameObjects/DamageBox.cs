using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.Collision;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CoffeeProject.GameObjects
{
    public class DamageBox : GameObject, IBodyComponent, ICollisionChecker<Hero>
    {
        private readonly DamageInstance _damage;
        public DamageBox(DamageInstance damage)
        {
            _damage = damage;
        }
        public bool Active { get; set; } = false;
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            if (Active)
            {
                obj.GetComponents<Dummy>().First().TakeDamage(_damage);
            }
        }

        public DamageBox Activate()
        {
            Active = true;
            return this;
        }
    }
}