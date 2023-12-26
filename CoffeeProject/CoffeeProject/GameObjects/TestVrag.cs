using BehaviorKit;
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
    internal class TestVrag : GameObject, IBodyComponent, ICollisionChecker<Hero>
    {
        public Vector2 Position { get ; set ; }
        public Rectangle Bounds { get ; set ; }

        public event OnDispose OnDisposeEvent;

        public void Dispose()
        {
            OnDisposeEvent(this);
        }

        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, Hero obj, Rectangle intersection)
        {
            var damageDict = new Dictionary<DamageType, int>();
            damageDict.Add(DamageType.Ice, 10);
            var damage = new DamageInstance(damageDict, Team.player, [], "tre", obj.GetComponents<Dummy>().First(), [], [], TimeSpan.FromSeconds(1));

            if (this.GetLayout().Intersects(obj.GetLayout()))
            {
                this.GetComponents<Dummy>().First().TakeDamage(damage);
            }
        }

        public TestVrag() 
        {
            var ye = new Dummy(100, [], Team.enemy, [], [], 1);
            this.CombineWith(ye);
        }
    }
}
