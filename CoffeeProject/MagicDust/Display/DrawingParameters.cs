using MagicDustLibrary.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace MagicDustLibrary.Display
{
    public partial record struct DrawingParameters : IPackable
    {
        public Vector2 Position;
        public Color Color;
        public float Rotation;
        public Vector2 Scale;
        public SpriteEffects Mirroring;
        public float Priority;

        public DrawingParameters()
        {
            Position = new Vector2(0, 0);
            Color = Color.White;
            Rotation = 0f;
            Scale = new Vector2(3, 3);
            Mirroring = SpriteEffects.None;
            Priority = 0;
        }

        public DrawingParameters
            (
            Vector2 position,
            Color color,
            float rotation,
            Vector2 scale,
            SpriteEffects mirroring,
            float priority
            )
        {
            Position = position;
            Color = color;
            Rotation = rotation;
            Scale = scale;
            Mirroring = mirroring;
            Priority = priority;
        }

        public static DrawingParameters operator +(DrawingParameters a, Vector2 b)
        {
            a.Position += b;
            return a;
        }

        public static DrawingParameters operator +(Vector2 b, DrawingParameters a)
        {
            return a + b;
        }

        public static DrawingParameters operator -(DrawingParameters a, Vector2 b)
        {
            return a + (-b);
        }

        public static DrawingParameters operator -(Vector2 b, DrawingParameters a)
        {
            return b + (-a);
        }

        public static DrawingParameters operator -(DrawingParameters a)
        {
            a.Position = -a.Position;
            return a;
        }
    }
}
