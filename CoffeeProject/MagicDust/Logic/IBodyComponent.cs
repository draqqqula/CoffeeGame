using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IBodyComponent : IDisposableComponent
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

        public static Rectangle PredictLayout(this IBodyComponent body, Vector2 movement)
        {
            return new
                Rectangle(
                (int)(body.Position.X + movement.X) + body.Bounds.X,
                (int)(body.Position.Y + movement.Y) + body.Bounds.Y,
                body.Bounds.Width,
                body.Bounds.Height
                );
        }

        public static bool Collides(this IBodyComponent a, IBodyComponent b)
        {
            return a.GetLayout().Contains(b.GetLayout());
        }
    }
}
