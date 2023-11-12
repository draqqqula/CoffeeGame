using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IBodyComponent : IGameObjectComponent
    {
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
    }

    public static class BodyExtensions
    {
        public static Rectangle GetLayout(this IBodyComponent body)
        {
            return new Rectangle(body.Position.ToPoint() + body.Bounds.Location, body.Bounds.Size);
        }
        public static bool Collides(this IBodyComponent a, IBodyComponent b)
        {
            return a.GetLayout().Contains(b.GetLayout());
        }
    }
}
