﻿using MagicDustLibrary.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.TileMap
{
    public enum FilterKind
    {
        HasAllTags,
        HasAnyTag,
        MatchTags
    }

    public class TileSheet
    {
        private readonly FrozenDictionary<byte, TileInfo> _tileDescriptions;
        private readonly Texture2D _sheet;
        public TileSheet(Texture2D sheet, params TileInfo[] tiles)
        {
            _sheet = sheet;
            _tileDescriptions = tiles.ToFrozenDictionary(it => it.Id, it => it);
        }

        public TileInfo GetInfo(byte id)
        {
            return _tileDescriptions[id];
        }

        private static Func<TileInfo, bool> BuildFilterTagFunction(FilterKind filterKind, string[] tags)
        {
            switch (filterKind)
            {
                case FilterKind.HasAllTags:
                    return it => tags.All(tag => it.HasTag(tag));
                case FilterKind.HasAnyTag:
                    return it => tags.Any(tag => it.HasTag(tag));
                case FilterKind.MatchTags:
                    return it => tags.Order().SequenceEqual(it.AllTags.Order());
                default:
                    return it => true;
            }
        }

        public IEnumerable<Point> UseFilter(FilterKind filterKind, byte[,] map, params string[] tags)
        {
            HashSet<byte> fittingBytes = _tileDescriptions.Values
                .Where(BuildFilterTagFunction(filterKind, tags))
                .Select(it => it.Id)
                .ToHashSet();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (fittingBytes.Contains(map[i, j]))
                    {
                        yield return new Point(map[i, j]);
                    }
                }
            }
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
            var tile = _tileDescriptions[id];
            var tilePosition = tile.Offset + frame * point;
            batch.Draw(
                        _sheet,
                        position + tilePosition.ToVector2(),
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
