using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Collision
{
    public interface ICollisionChecker<T> : ICollisionChecker where T : IBodyComponent
    {
        public void OnCollisionWith(IControllerProvider state, TimeSpan deltaTime, T obj, Rectangle intersection);
    }

    public interface ICollisionChecker : IBodyComponent
    {
    }
}
