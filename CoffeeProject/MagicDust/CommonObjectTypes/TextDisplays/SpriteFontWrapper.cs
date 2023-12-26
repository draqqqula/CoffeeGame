using MagicDustLibrary.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MagicDustLibrary.CommonObjectTypes.TextDisplays
{
    public class SpriteFontWrapper
    {
        public SpriteFont Font { get; init; }
        public SpriteFontWrapper(
            [FromStorage("SpriteFonts", "*")] SpriteFont font
            )
        {
            Font = font;
        }
    }
}
