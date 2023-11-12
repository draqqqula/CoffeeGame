using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes
{
    public class SimpleText : GameObject
    {
        public string Text { get; set; } = "text";

        public SpriteFont Font { get; set; }

        public float TextScale { get; set; } = 1;

        public override IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            throw new NotImplementedException();
        }
    }

    public record struct TextDisplay : IDisplayable
    {
        private readonly DrawingParameters _arguments;

        private readonly string _text;

        private readonly int _fontId;

        private readonly float _fextScale;

        public void Draw(SpriteBatch batch, GameCamera camera, IContentStorage contentStorage)
        {
            batch.DrawString(contentStorage.GetAsset<SpriteFont>(_fontId), _text,
                _arguments.Position,
                _arguments.Color, 0, Vector2.Zero, _fextScale, _arguments.Mirroring, 0);
        }

        public TextDisplay()
        {

        }
    }
}
