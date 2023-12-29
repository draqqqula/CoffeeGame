using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.TextDisplays
{
    public enum PivotPosition
    {
        TopLeft,
        Center,
        CenterLeft
    }

    public class Label : GameObject, IDisplayComponent, IBodyComponent
    {
        private static class TextConstants
        {
            public const string DEFAULT_TEXT = "Text";
        }

        private SpriteFont? _font;

        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
        public string Text { get; set; } = TextConstants.DEFAULT_TEXT;
        public float Scale { get; set; } = 1f;
        public Color Color { get; set; } = Color.White;
        public PivotPosition Pivot {  get; set; } = PivotPosition.TopLeft;

        public event OnDispose OnDisposeEvent = delegate { };

        public void Dispose()
        {
            OnDisposeEvent(this);
        }

        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            if (_font is null)
            {
                yield break;
            }
            yield return new StaticTextDisplay
            {
                Text = Text,
                Font = _font,
                Parameters = layer.Process(GetDrawingParameters(), camera)
            };
        }

        public DrawingParameters GetDrawingParameters()
        {
            return new DrawingParameters
            {
                Position = DisplayPosition,
                Scale = new Vector2(Scale, Scale),
                Color = Color
            };
        }

        private Vector2 DisplayPosition
        {
            get
            {
                switch (Pivot)
                {
                    case PivotPosition.TopLeft:
                        return Position;
                    case PivotPosition.Center:
                        return Position + new Vector2(Bounds.Left, Bounds.Top);
                    case PivotPosition.CenterLeft:
                        return Position + new Vector2(0, Bounds.Top);
                    default:
                        return Vector2.Zero;
                }
            }
        }

        internal Label UseFont(IControllerProvider state, string fileName)
        {
            _font = state.Using<IFactoryController>().CreateAsset<SpriteFontWrapper>(fileName).Font;
            return this;
        }

        internal Label UseCustomFont(IControllerProvider state, string fileName)
        {
            _font = state.Using<IFactoryController>().CreateAsset<CustomFontWrapper>(fileName).Font;
            return this;
        }

        internal Label SetColor(Color color)
        {
            Color = color;
            return this;
        }

        internal Label SetPivot(PivotPosition pivot)
        {
            Pivot = pivot;
            return this;
        }

        internal Label SetText(string text)
        {
            if (text != Text)
            {
                Text = text;
                if (_font is not null)
                {
                    var size = _font.MeasureString(Text) * Scale;
                    if (Pivot == PivotPosition.TopLeft)
                    {
                        Bounds = new Rectangle(Point.Zero, size.ToPoint());
                    }
                    else if (Pivot == PivotPosition.Center)
                    {
                        Bounds = new Rectangle((-size / 2).ToPoint(), size.ToPoint());
                    }
                    else if (Pivot == PivotPosition.CenterLeft)
                    {
                        Bounds = new Rectangle((-new Vector2(0, size.Y) / 2).ToPoint(), size.ToPoint());
                    }
                }
            }
            return this;
        }

        internal Label SetScale(float scale)
        {
            Scale = scale;
            return this;
        }
    }

    public struct StaticTextDisplay : IDisplayable
    {
        public SpriteFont Font { get; init; }
        public string Text { get; init; }
        public DrawingParameters Parameters { get; init; }
        public IComparable OrderComparer => Parameters.OrderComparer;

        public void Draw(SpriteBatch batch, GameCamera camera, IContentStorage contentStorage)
        {
            batch.DrawString(Font, 
                Text, 
                Parameters.Position, 
                Parameters.Color, 
                Parameters.Rotation, 
                Vector2.Zero, 
                Parameters.Scale, 
                Parameters.Mirroring, 0);
        }
    }

    public static class LabelExtensions
    {
        public static T SetText<T>(this T obj, string text) where T : Label
        {
            return obj.SetText(text) as T;
        }

        public static T UseFont<T>(this T obj, IControllerProvider state, string fileName) where T : Label
        {
            return obj.UseFont(state, fileName) as T;
        }

        public static T UseCustomFont<T>(this T obj, IControllerProvider state, string fileName) where T : Label
        {
            return obj.UseCustomFont(state, fileName) as T;
        }

        public static T SetScale<T>(this T obj, float scale) where T : Label
        {
            return obj.SetScale(scale) as T;
        }

        public static T SetColor<T>(this T obj, Color color) where T : Label
        {
            return obj.SetColor(color) as T;
        }

        public static T SetPivot<T>(this T obj, PivotPosition pivot) where T : Label
        {
            return obj.SetPivot(pivot) as T;
        }
    }
}
