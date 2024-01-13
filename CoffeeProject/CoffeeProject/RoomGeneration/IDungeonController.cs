using CoffeeProject.Encounters;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes.TileMap;
using MagicDustLibrary.Content;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using Microsoft.Xna.Framework;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MagicDustLibrary.Organization;
using CoffeeProject.GameObjects;
using CoffeeProject.SurfaceMapping;
using CoffeeProject.BoxDisplay;
using CoffeeProject.Collision;
using MagicDustLibrary.Organization.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeProject.RoomGeneration
{
    public interface IDungeonController : IStateController
    {
        public void CreateDungeon(DungeonParameters args, TileMapParameters tileArgs, EncounterMapper mapper);
        public IEnumerable<Room> Rooms { get; }
    }

    public class DungeonController : IDungeonController
    {
        private readonly IControllerProvider _state;
        private Room[] _rooms;
        public IEnumerable<Room> Rooms => _rooms;
        public DungeonController(IControllerProvider state)
        {
            _state = state;
        }

        public void CreateDungeon(DungeonParameters generatorArgs, TileMapParameters tileArgs, EncounterMapper mapper)
        {
            var sheet = _state.Using<IFactoryController>().CreateAsset<TileSheet>(tileArgs.TileAsset);

            var generator = new LevelGenerator(_state);
            var graph = generator.GenerateLevelGraph(
                generatorArgs.AssetName, 
                generatorArgs.MainPathLength, 
                generatorArgs.EnemyRoomCount, 
                generatorArgs.LootRoomCount
                );

            var foreground = CreateMap<SurfaceLayer>(sheet, tileArgs.FrameSize, tileArgs.Scale, graph.LevelColors);

            var background = CreateMap<FloorLayer>(sheet, tileArgs.FrameSize, tileArgs.Scale, graph.BackgroundColors);

            _rooms = BuildRooms(graph, foreground).ToArray();

            _state.Using<SurfaceMapProvider>().AddMap(tileArgs.SurfaceMapName, foreground);

            mapper.InvokeAll(_state, graph, _rooms, point => foreground.Position + point.ToVector2() * foreground.CellSize);
        }


        private IEnumerable<Room> BuildRooms(GraphInfo graph, TileMap map)
        {
            foreach (var roomLayout in graph.Positions.Values)
            {
                var trigger = _state.Using<IFactoryController>().CreateObject<RoomTrigger>()
                    .SetPos(roomLayout.Location.ToVector2() * map.CellSize + map.Position)
                    .SetBounds(new Rectangle(Point.Zero, (roomLayout.Size.ToVector2() * map.CellSize).ToPoint()))
                    .AddToState(_state);
                var room = new Room(trigger);
                room.Trigger.PlayerDetected += (player) => AlertEnemies(player, room);
                yield return room;
            }
        }


        private void AlertEnemies(Hero player, Room room)
        {
            foreach (var enemy in room.Enemies)
            {
                enemy.SetTarget(_state, player);
            }
        }


        private TileMap CreateMap<L>(TileSheet sheet, int frameSize, float scale, Color[,] data) where L : Layer
        {
            var map = _state.Using<IFactoryController>()
            .CreateObject<TileMap>()
            .SetPlacement(new Placement<L>());

            map.SetFrame(new Point(frameSize, frameSize));
            map.SetScale(scale);
            map.UseSheet(sheet);

            var backgroundLevel = new LevelMap(data);
            map.UseMap(backgroundLevel.Map);
            map.AddToState(_state);
            return map;
        }
    }

    public record class DungeonParameters(string AssetName, int MainPathLength, int EnemyRoomCount, int LootRoomCount);
    public record class TileMapParameters(string TileAsset, int FrameSize, float Scale, string SurfaceMapName);

    public static class DungeonExtensions
    {
        public static void ConfigureDungeonController(IServiceCollection services, LevelSettings settings)
        {
            services.AddSingleton<IDungeonController, DungeonController>();
        }
    }
}
