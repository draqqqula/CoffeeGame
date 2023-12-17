using MagicDustLibrary.Common;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Network;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicDustLibrary.CommonObjectTypes.TileMap
{
    public partial class TileMap : NodeComponent, IDisplayComponent, IBodyComponent
    {
        private byte[,] _map = new byte[0, 0];
        private float _scale = 1;
        private TileSheet? _sheet;
        private IEnumerable<Point> ExtraPoints = [];
        private Point _tileFrame = new Point(36, 36);

        public Rectangle Bounds
        {
            get
            {
                return
                    new Rectangle(
                        new Point(0, 0),
                        new Vector2(_map.GetLength(0) * _tileFrame.X * _scale, _map.GetLength(1) * _tileFrame.Y * _scale).ToPoint()
                        );
            }
            set
            {

            }
        }

        public Vector2 Position { get; set; }

        public void UseSheet(TileSheet sheet)
        {
            _sheet = sheet;
            ExtraPoints = sheet.UseFilter(FilterKind.HasAnyTag, _map, "special");
        }

        public void UseMap(byte[,] map)
        {
            _map = map;

            if (_sheet is null)
            {
                return;
            }
            ExtraPoints = _sheet.UseFilter(FilterKind.HasAnyTag, map, "special");
        }

        public void SetFrame(Point tileFrame)
        {
            _tileFrame = tileFrame;
        }

        public void SetScale(float scale)
        {
            _scale = scale;
        }

        public Rectangle? GetBoundsForPoint(Point point)
        {
            if (_sheet is null ||
                _map.GetLength(0) > point.X ||
                _map.GetLength(1) > point.Y ||
                point.X < 0 ||
                point.Y < 0)
            {
                return null;
            }
            var tile = _sheet.GetInfo(_map[point.X, point.Y]);

            var rawPosition = _tileFrame * point + tile.Offset;
            var position = Position + rawPosition.ToVector2() * _scale;
            var size = tile.Bounds.Size.ToVector2() * _scale;

            return new Rectangle(position.ToPoint(), size.ToPoint());
        }

        private Rectangle GetChunk(Rectangle window)
        {
            if (Bounds.Size == Point.Zero)
            {
                return Rectangle.Empty;
            }
            float width = _map.GetLength(0);
            float height = _map.GetLength(1);
            var frame = _tileFrame.ToVector2() * new Vector2(_scale, _scale);
            var startX = (int)Math.Clamp(window.Left / frame.X, 0f, width);
            var startY = (int)Math.Clamp(window.Top / frame.Y, 0f, height);
            var endX = (int)Math.Ceiling(Math.Clamp(window.Right / frame.X, 0f, width));
            var endY = (int)Math.Ceiling(Math.Clamp(window.Bottom / frame.Y, 0f, height));
            return new Rectangle(startX, startY, endX - startX, endY - startY);
        }

        public void DrawChunk(SpriteBatch batch, Rectangle chunk, Vector2 position)
        {
            if (_sheet is null)
            {
                return;
            }
            _sheet.DrawMapChunk(batch, ref _map, chunk, _tileFrame, position, _scale);
        }

        public void DrawPoints(SpriteBatch batch, Vector2 position, IEnumerable<Point> points)
        {
            if (_sheet is null)
            {
                return;
            }
            foreach (var point in points)
            {
                _sheet.DrawAtPoint(batch,
                    _map[point.X, point.Y],
                    point,
                    _tileFrame,
                    position,
                    _scale);
            }
        }

        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            var pos = layer.Process(GetDrawingParameters(), camera).Position;
            var chunk = GetChunk(new Rectangle((-pos).ToPoint(), camera.ViewPort.Size));
            var extras = ExtraPoints.Where(it => camera.ViewPort.Intersects(GetBoundsForPoint(it) ?? Rectangle.Empty));
            yield return new TileMapChunk(this, chunk, pos, extras);
        }

        public DrawingParameters GetDrawingParameters()
        {
            var info = new DrawingParameters()
            {
                Position = this.Position,
                Mirroring = SpriteEffects.None,
            };
            return info;
        }

    }
}
