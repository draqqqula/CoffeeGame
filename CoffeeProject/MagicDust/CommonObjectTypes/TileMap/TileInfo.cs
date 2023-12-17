using Microsoft.Xna.Framework;

namespace MagicDustLibrary.CommonObjectTypes.TileMap
{
    public record struct TileInfo
    {
        private readonly HashSet<string> _tags = [];

        public TileInfo(byte id, Rectangle bounds) : this(id, bounds, [])
        {
        }

        public TileInfo(byte id, Rectangle bounds, params string[] tags) : this(id, bounds, Point.Zero, tags)
        {
        }

        public TileInfo(byte id, Rectangle bounds, Point offset, params string[] tags)
        {
            Id = id;
            Bounds = bounds;
            Offset = offset;
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
        }

        public Rectangle Bounds { get; init; }
        public Point Offset { get; init; }
        public byte Id { get; init; }

        public void AddTag(string tag)
        {
            _tags.Add(tag);
        }

        public bool HasTag(string tag)
        {
            return _tags.Contains(tag);
        }

        public readonly IEnumerable<string> AllTags => _tags;
    }
}
