using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.BoxDisplay;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.RoomGeneration;
using CoffeeProject.Run;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.CommonObjectTypes.TileMap;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    public class TestLevel3 : GameLevel
    {
        protected override LevelSettings GetDefaults()
        {
            var settings = new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
            return settings;
        }

        Vector2 PlayerPosition { get; set; } = Vector2.Zero;
        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            var generator = new LevelGenerator(state);
            var graph = generator.GenerateLevelGraph("TestLevel", 4, 8, 8);

            var map = state.Using<IFactoryController>()
                .CreateObject<TileMap>()
                .SetPos(new Vector2(-500, -500))
                .SetPlacement(new Placement<SurfaceLayer>());
            map.SetFrame(new Point(324, 324));
            map.SetScale(0.2f);
            var sheet = state.Using<IFactoryController>().CreateAsset<TileSheet>("level1");
            map.UseSheet(sheet);
            var level = new LevelMap(graph.LevelColors);
            map.UseMap(level.Map);
            map.AddToState(state);

            var backgroundMap = state.Using<IFactoryController>()
                .CreateObject<TileMap>()
                .SetPos(new Vector2(-500, -500))
                .SetPlacement(new Placement<FloorLayer>());
            backgroundMap.SetFrame(new Point(324, 324));
            backgroundMap.SetScale(0.2f);
            backgroundMap.UseSheet(sheet);
            var backgroundLevel = new LevelMap(graph.BackgroundColors);
            backgroundMap.UseMap(backgroundLevel.Map);
            backgroundMap.AddToState(state);


            state.Using<SurfaceMapProvider>().AddMap("level", map);

            PlayerPosition = map.GetBoundsForPoint(graph.Positions.First().Value.Location).Value.Location.ToVector2() + new Vector2(300, 300);

            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");
            var enemyPos = map.GetBoundsForPoint(graph.Positions[1].Location).Value.Location.ToVector2() + new Vector2(300, 300);
            Enemy = state.Using<IFactoryController>()
                .CreateObject<NaughtyShell>()
                .SetPos(enemyPos)
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddToState(state);
            Enemy.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);
        }
        private NaughtyShell Enemy { get; set; }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");

            var healthIndicator = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(100, 50))
                .UseCustomFont(state, "TestFont")
                .SetScale(4f)
                .SetText("c")
                .AddToState(state);

            var obj = state.Using<IFactoryController>().CreateObject<Hero>()
                .SetPos(PlayerPosition)
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddComponent(new TimerHandler())
                .AddComponent(new Playable(healthIndicator))
                .AddShadow(state)
                .AddToState(state);


            Enemy.SetTarget(state, obj);

            obj.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);
            var dummy = obj.GetComponents<Dummy>().First();

            obj.Client = client;

            state.Using<IClientController>().AttachCamera(client, obj);
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
        }
    }
}
