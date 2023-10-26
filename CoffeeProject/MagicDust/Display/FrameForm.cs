using MagicDustLibrary.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicDustLibrary.Display
{
    public partial struct FrameForm : IDisplayable, IPackable
    {
        #region FIELDS
        private readonly Rectangle Borders;
        private readonly Vector2 Anchor;
        private readonly DrawingParameters Arguments;
        private readonly Texture2D Sheet;
        #endregion

        #region CONSTRUCTORS
        public FrameForm(Rectangle borders, Vector2 anchor, DrawingParameters arguments, Texture2D sheet)
        {
            Borders = borders;
            Anchor = anchor;
            Arguments = arguments;
            Sheet = sheet;
        }
        #endregion

        #region IDISPLAYABLE
        public void Draw(SpriteBatch spriteBatch, GameCamera camera, IContentStorage contentStorage)
        {
            Vector2 offset = Vector2.Zero;
            if (Arguments.Mirroring == SpriteEffects.FlipHorizontally)
                offset = new Vector2(Anchor.X * 2 - Borders.Width, 0) * Arguments.Scale;
            spriteBatch.Draw(
                Sheet,
                Arguments.Position + offset,
                Borders,
                Arguments.Color,
                Arguments.Rotation,
                Anchor,
                Arguments.Scale,
                Arguments.Mirroring,
                new Random().NextSingle()
                );
        }
        #endregion
    }
}
