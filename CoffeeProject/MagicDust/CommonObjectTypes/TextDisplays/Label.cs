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
                Position = Position,
                Scale = new Vector2(Scale, Scale),
                Color = Color
            };
        }

        public Label UseFont(IControllerProvider state, string fileName)
        {
            _font = state.Using<IFactoryController>().CreateAsset<SpriteFontWrapper>(fileName).Font;
            return this;
        }

        public Label UseCustomFont(IControllerProvider state, string fileName)
        {
            _font = state.Using<IFactoryController>().CreateAsset<CustomFontWrapper>(fileName).Font;
            return this;
        }

        public Label SetText(string text)
        {
            Text = text;
            Bounds = new Rectangle(Point.Zero, (_font.MeasureString(Text) * Scale).ToPoint());
            return this;
        }

        public Label SetScale(float scale)
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
}
