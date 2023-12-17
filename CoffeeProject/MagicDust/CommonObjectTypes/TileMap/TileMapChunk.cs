using MagicDustLibrary.Display;
using MagicDustLibrary.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.TileMap
{
    public partial struct TileMapChunk : IDisplayable, IPackable
    {
        private TileMap Source;
        private Vector2 Position;
        private Rectangle Chunk;
        private IEnumerable<Point> Extra;

        public void Draw(SpriteBatch batch, GameCamera camera, IContentStorage contentStorage)
        {
            Source.DrawChunk(batch, Chunk, Position);
            Source.DrawPoints(batch, Position, Extra);
        }

        public TileMapChunk(TileMap source, Rectangle chunk, Vector2 position, IEnumerable<Point> extra)
        {
            Source = source;
            Chunk = chunk;
            Position = position;
            Extra = extra;
        }
    }
}
