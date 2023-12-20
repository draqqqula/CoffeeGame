using MagicDustLibrary.CommonObjectTypes.TileMap;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Organization.DefualtImplementations;
using Microsoft.Extensions.DependencyInjection;
using RectangleFLib;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;

namespace CoffeeProject.SurfaceMapping
{
    public class SurfaceMapStorage
    {
        private readonly Dictionary<string, SurfaceMap> _maps = [];

        public SurfaceMap GetMap(string key)
        {
            return _maps[key];
        }

        public void AddMap(string key, SurfaceMap map)
        {
            _maps.Add(key, map);
        }

        public void AddMap(string key, TileMap map)
        {
            var points = map.FilterMap(TileMap.FilterKind.HasAnyTag, "solid");
            var surfaces = points
                .GroupBy(it => it.X)
                .ToDictionary(it => it.Key, it => MakeSurfaces(it.OrderBy(it => it.Y), map.Position, map.CellSize).ToArray());

            AddMap(key, new SurfaceMap(surfaces, map.MapLengthX, map.CellSize.X).SetPos(map.Position));
        }

        private static IEnumerable<RectangleF> MakeSurfaces(IEnumerable<Point> pointsSorted, Vector2 position, Vector2 size)
        {
            if (pointsSorted.Count() == 0)
            {
                yield break;
            }
            var highest = pointsSorted.First();
            var result = new List<Point>() { highest };
            int previousY = highest.Y;
            int previousX = highest.X;
            int height = 1;
            foreach (var point in pointsSorted.Skip(1))
            {
                if (point.Y == previousY + 1)
                {
                    height++;
                    continue;
                }
                yield return new RectangleF(
                    position.X + size.X * previousX,
                    position.Y + size.Y * previousY,
                    size.X,
                    size.Y * height);
                previousY = point.Y;
                previousX = point.X;
                height = 1;
            }
            yield return new RectangleF(
                    position.X + size.X * previousX,
                    position.Y + size.Y * previousY,
                    size.X,
                    size.Y * height);
        }
    }

    public class SurfaceMapProvider : IStateController
    {
        private readonly SurfaceMapStorage _storage;
        public SurfaceMapProvider(SurfaceMapStorage storage)
        {
            _storage = storage;
        }

        public SurfaceMap GetMap(string key)
        {
            return _storage.GetMap(key);
        }

        public void AddMap(string key, TileMap map)
        {
            _storage.AddMap(key, map);
        }
    }

    public static class SurfacesExtensions
    {
        public static void ConfigureCustomSurfaceServices(IServiceCollection services, LevelSettings settings)
        {
            services.AddSingleton<SurfaceMapStorage>();
            services.AddSingleton<SurfaceMapProvider>();
        }
    }

    public class SurfaceMap : ComponentBase, IBodyComponent
    {
        public SurfaceMap(Dictionary<int, RectangleF[]> verticalSlices, int length, float cellWidth)
        {
            _verticalSlices = verticalSlices;
            Length = length;
            CellWidth = cellWidth;
        }

        private readonly Dictionary<int, RectangleF[]> _verticalSlices;
        public float CellWidth { get; init; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds
        {
            get; set;
        }
        public int Length { get; init; }

        public IEnumerable<RectangleF> GetSpan(int start, int count)
        {
            return Enumerable.Range(start, count)
                .Select(it => _verticalSlices.TryGetValue(it, out var rect) ? rect: [])
                .SelectMany(it => it);
        }

        public event OnDispose OnDisposeEvent;

        public void Dispose()
        {
            OnDisposeEvent(this);
        }
    }
}