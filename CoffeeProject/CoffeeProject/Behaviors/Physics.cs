using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using RectangleFLib;

namespace CoffeeProject.Behaviors
{
    public static class SideExtensions
    {
        public static Point ToPoint(this Side side)
        {
            switch (side)
            {
                case Side.Left:
                    return new Point(-1, 0);
                case Side.Right:
                    return new Point(1, 0);
                case Side.Top:
                    return new Point(0, -1);
                case Side.Bottom:
                    return new Point(0, 1);
                default:
                    return new Point(0, 0);
            }
        }

        public static Point ToPoint(this Direction direction)
        {
            return direction.ToSide().ToPoint();
        }

        public static Direction ToDirection(this Side side)
        {
            switch (side)
            {
                case Side.Left:
                    return Direction.Left;
                case Side.Right:
                    return Direction.Right;
                case Side.Top:
                    return Direction.Backward;
                default:
                    return Direction.Forward;
            }
        }

        public static Side ToSide(this Direction side)
        {
            switch (side)
            {
                case Direction.Left:
                    return Side.Left;
                case Direction.Right:
                    return Side.Right;
                case Direction.Backward:
                    return Side.Top;
                default:
                    return Side.Bottom;
            }
        }
    }

    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum Direction
    {
        Left,
        Right,
        Forward,
        Backward
    }

    /// <summary>
    /// описывает физическое воздействие на объект в определённом направлении
    /// </summary>
    public class MovementVector
    {
        public Vector2 Vector
        {
            get
            {
                return Direction * Module;
            }
        }
        public float Module;
        public Vector2 Direction;
        public float Acceleration;
        public float MaxModule;
        public float MinModule;
        private readonly float OriginalModule;
        public bool Enabled;

        public TimeSpan LifeTime;
        public TimeSpan LivingTime;
        public bool Immortal;

        public void Originalize()
        {
            Module = OriginalModule;
        }

        public void Direct(Vector2 directon)
        {
            directon.Normalize();
            Direction = directon;
        }

        public bool Update(TimeSpan deltaTime)
        {
            if (Enabled)
            {
                if (Acceleration != 0)
                {
                    Module = Math.Clamp(Module + Acceleration * (float)deltaTime.TotalSeconds, MinModule, MaxModule);
                    if (Module <= 0)
                    {
                        return false;
                    }
                }

                if (!Immortal)
                {
                    LifeTime += deltaTime;
                    if (LifeTime >= LivingTime)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        public MovementVector(Vector2 vector, float acceleration, TimeSpan livingTime, bool immortal) :
            this(
                MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y),
                vector / MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y),
                acceleration,
                livingTime,
                immortal,
                float.MaxValue,
                0,
                true
                )
        {
        }

        public MovementVector(float module, Vector2 direction, float acceleration, TimeSpan livingTime, bool immortal, float maxModule, float minModule, bool enabled)
        {
            Module = module;
            Direction = direction;
            Acceleration = acceleration;
            Enabled = enabled;
            LivingTime = livingTime;
            Immortal = immortal;
            MaxModule = maxModule;
            MinModule = minModule;
            OriginalModule = module;
        }

        public MovementVector(Vector2 vector, float acceleration, float maxModule, TimeSpan livingTime, bool immortal) :
            this(
                MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y),
                vector / MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y),
                acceleration,
                livingTime,
                immortal,
                float.MaxValue,
                float.MinValue,
                true
                )
        {
        }
    }

    /// <summary>
    /// Описывает поведение объекта, подверженного физике
    /// </summary>
    public class Physics : Behavior<IBodyComponent>
    {
        public SurfaceMap SurfaceMap { get; set; }
        public float SurfaceWidth => SurfaceMap.CellWidth;

        public Dictionary<Side, bool> Faces;
        public Dictionary<string, MovementVector> Vectors { get; private set; }

        public Vector2 GetResultingVector(TimeSpan deltaTime)
        {
            return HalfUpdateVectors(deltaTime) * 2;
        }

        public Dictionary<string, MovementVector> ActiveVectors
        {
            get => Vectors.Where(v => v.Value.Enabled).ToDictionary(e => e.Key, e => e.Value);
        }

        public void OriginalizeVector(string name)
        {
            Vectors[name].Originalize();
        }

        public void DirectVector(string name, Vector2 direction)
        {
            Vectors[name].Direct(direction);
        }

        public void AddVector(string name, MovementVector vector)
        {
            Vectors[name] = vector;
        }

        public void RemoveVector(string name)
        {
            Vectors.Remove(name);
        }

        public IEnumerable<RectangleF> GetMapSegment(int start, int end)
        {
            var relativeStart = start - SurfaceMap.Position.X;
            var relativeEnd = end - SurfaceMap.Position.X;
            var imaginaryStart = Math.Min(Math.Max((int)Math.Floor(relativeStart / SurfaceWidth), 0), SurfaceMap.Length);
            var imaginaryEnd = Math.Max(0, Math.Min((int)Math.Ceiling(relativeEnd / SurfaceWidth) + 1, SurfaceMap.Length));
            return SurfaceMap.GetSpan(imaginaryStart, imaginaryEnd - imaginaryStart);
        }

        private Vector2 ApplyCollision(Rectangle start, IEnumerable<RectangleF> surfaces, Rectangle end)
        {
            foreach (Side side in (Side[])Enum.GetValues(typeof(Side)))
                Faces[side] = false;
            var moving = new Rectangle(end.Location, end.Size);



            var surfaceList = surfaces.ToList();
            while (surfaceList.Count > 0)
            {
                var surface = surfaceList.MaxBy(r => { var i = Rectangle.Intersect(moving, r); return i.Width * i.Height; });

                var intersection = Rectangle.Intersect(moving, surface);
                if (!intersection.IsEmpty)
                {
                    var distance = moving.Center - intersection.Center;

                    if (distance == Point.Zero)
                    {
                        return Vector2.Zero;
                    }

                    if (intersection.Width > intersection.Height)
                    {
                        var factor = distance.Y / Math.Abs(distance.Y);

                        if (factor > 0) Faces[Side.Top] = true;
                        else Faces[Side.Bottom] = true;

                        moving.Offset(0, intersection.Height * factor);
                    }
                    else if (intersection.Width < intersection.Height)
                    {
                        var factor = distance.X / Math.Abs(distance.X);

                        if (factor > 0) Faces[Side.Right] = true;
                        else Faces[Side.Left] = true;

                        moving.Offset(intersection.Width * factor, 0);
                    }
                }
                surfaceList.Remove(surface);
            }
            return (moving.Location - end.Location).ToVector2();
        }

        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IBodyComponent parent)
        {
            Vector2 resultingVector = HalfUpdateVectors(deltaTime);
            var resultingLength = resultingVector.Length();
            var direction = Vector2.Normalize(resultingVector);
            var allowedSpeed = SurfaceWidth / 4;

            for (int i = 0; i < Math.Ceiling(resultingVector.Length() / allowedSpeed); i++)
            {
                var sequenceLength = Math.Min(allowedSpeed, resultingLength - i * allowedSpeed);
                var vectorSequence = direction * sequenceLength;

                var pastPosition = GetLayoutF(parent.Bounds, parent.Position);
                var futurePosition = PredictLayout(pastPosition, vectorSequence);
                var mapSegment = GetMapSegment(
                    Convert.ToInt32(Math.Floor(Math.Min(pastPosition.Left, futurePosition.Left))), 
                    Convert.ToInt32(Math.Floor(Math.Max(pastPosition.Right, futurePosition.Right))));
                var collisionFactor = ApplyCollision(pastPosition, mapSegment, futurePosition);

                parent.Position += vectorSequence + collisionFactor;
            }

            HalfUpdateVectors(deltaTime);
        }

        private static RectangleF GetLayoutF(Rectangle bounds, Vector2 position)
        {
            return new RectangleF(bounds.X + position.X, bounds.Y + position.Y, bounds.Width, bounds.Height);
        }

        private static RectangleF PredictLayout(RectangleF layout, Vector2 vector)
        {
            return new RectangleF(layout.Location.X + vector.X, layout.Location.Y + vector.Y, layout.Width, layout.Height);
        }

        private Vector2 HalfUpdateVectors(TimeSpan deltaTime)
        {
            Vector2 resultingVector = Vector2.Zero;
            foreach (var vector in ActiveVectors)
            {
                resultingVector += vector.Value.Vector * (float)(deltaTime.TotalSeconds * 60);
                if (!vector.Value.Update(deltaTime / 2))
                    Vectors.Remove(vector.Key);
            }
            return resultingVector;
        }

        public Physics(SurfaceMap surfaceMap) : base()
        {
            SurfaceMap = surfaceMap;
            Vectors = new Dictionary<string, MovementVector>();

            Faces = new Dictionary<Side, bool>();
            foreach (Side side in (Side[])Enum.GetValues(typeof(Side)))
                Faces[side] = false;
        }
    }
}
