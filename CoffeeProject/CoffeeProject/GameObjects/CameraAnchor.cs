using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    internal class CameraAnchor : GameObject, IBodyComponent, IMultiBehaviorComponent, IUpdateComponent
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct;

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            OnAct(state, deltaTime, this);
        }
    }
}
