using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IBody
    {
        public void SetPosition(Vector2 position);
        public Vector2 GetPosition();
        public Rectangle GetBounds();
        public void SetBounds(Rectangle box);
    }

    public static class BodyExtensions
    {
        public static Rectangle GetLayout(this IBody body)
        {
            return new Rectangle(body.GetPosition().ToPoint() + body.GetBounds().Location, body.GetBounds().Size);
        }
        public static bool Collides(this IBody a, IBody b)
        {
            return a.GetLayout().Contains(b.GetLayout());
        }
    }
}
