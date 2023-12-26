using MagicDustLibrary.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Extensions;

namespace MagicDustLibrary.CommonObjectTypes.TextDisplays
{
    public class CustomFontWrapper
    {
        private class CharacterInfo
        {
            public char Character { get; set; }
            public Rectangle Bounds { get; set; }
            public Rectangle Cropping { get; set; }
            public Vector3 Kerning { get; set; }
        }

        public SpriteFont Font { get; init; }

        public CustomFontWrapper(
            [FromStorage("SpriteFonts", "*", "texture")]Texture2D texture,
            [FromStorage("SpriteFonts", "*", "info")]ExpandoObject info
            )
        {
            var charList = ParseCharacterList(info).OrderBy(it => it.Character);
            Font = new SpriteFont(
                texture, 
                charList.Select(it => it.Bounds).ToList(),
                charList.Select(it => it.Cropping).ToList(),
                charList.Select(it => it.Character).ToList(),
                GetLineSpacing(info), 
                GetSpacing(info),
                charList.Select(it => it.Kerning).ToList(),
                charList.First().Character);
        }

        private static IEnumerable<CharacterInfo> ParseCharacterList(dynamic json)
        {
            List<object> charList = json.CharList;
            return charList.Select(it => ParseCharacterInfo(it));
        }

        private static int GetLineSpacing(dynamic json)
        {
            long value = json.LineSpacing;
            return Convert.ToInt32(value);
        }

        private static float GetSpacing(dynamic json)
        {
            string value = json.Spacing;
            return Convert.ToSingle(value);
        }

        private static CharacterInfo ParseCharacterInfo(dynamic charInfo)
        {
            var result = new CharacterInfo();
            string character = charInfo.Character;
            result.Character = character.First();
            result.Bounds = MagicJsonExtensions.ReadRectangle(charInfo.Bounds);
            result.Cropping = MagicJsonExtensions.ReadRectangle(charInfo.Cropping);
            result.Kerning = MagicJsonExtensions.ReadVector3(charInfo.Kerning);
            return result;
        }
    }
}
