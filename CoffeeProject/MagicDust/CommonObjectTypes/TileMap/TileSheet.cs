using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.TileMap
{
    public class TileSheet
    {
        private readonly Texture2D _sheet;
        public TileSheet(Texture2D sheet, params TileInfo[] tiles)
        {
            _sheet = sheet;
            TileDescriptions = tiles.ToFrozenDictionary(it => it.Id, it => it);
        }

        public FrozenDictionary<byte, TileInfo> TileDescriptions { get; init; }

        public TileSheet(
            [FromContent("*_folder", "sheet")]Texture2D sheet,
            [FromContent("*_folder", "info")]List<ExpandoObject> tiles
            ) :
            this(sheet, ParseTileInfo(tiles).ToArray())
        {

        }

        private static IEnumerable<TileInfo> ParseTileInfo(List<ExpandoObject> json)
        {
            foreach (dynamic value in json)
            {
                List<object> tags = value.Tags;
                List<object> bounds = value.Bounds;
                List<object> offset = value.Offset;
                long id = value.Id;
                yield return new TileInfo(
                    (byte)id,
                    new Rectangle(Convert.ToInt32(bounds[0]), Convert.ToInt32(bounds[1]), Convert.ToInt32(bounds[2]), Convert.ToInt32(bounds[3])),
                    new Point(Convert.ToInt32(offset[0]), Convert.ToInt32(offset[1])), tags.Select(it => (string)it).ToArray());
            }
        }

        public TileInfo GetInfo(byte id)
        {
            return TileDescriptions[id];
        }

        public void DrawMapChunk(SpriteBatch batch, ref byte[,] map, Rectangle chunk, Point frame, Vector2 position, float scale)
        {
            for (int i = chunk.Left; i < chunk.Right; i++)
            {
                for (int j = chunk.Top; j < chunk.Bottom; j++)
                {
                    DrawAtPoint(batch, map[i, j], new Point(i, j), frame, position, scale);
                }
            }
        }

        public void DrawAtPoint(SpriteBatch batch, byte id, Point point, Point frame, Vector2 position, float scale)
        {
            if (id == 0)
            {
                return;
            }    
            var tile = TileDescriptions[id];
            var tilePosition = tile.Offset + frame * point;
            batch.Draw(
                        _sheet,
                        position + tilePosition.ToVector2()*scale,
                        tile.Bounds,
                        Color.White,
                        0,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        0
                    );
        }
    }
}
