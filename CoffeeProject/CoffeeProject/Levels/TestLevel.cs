using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.BoxDisplay;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    public class TestLevel : GameLevel
    {
        protected override LevelSettings GetDefaults()
        {
            var settings = new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
            return settings;
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            //state.OpenServer(7878);
            var map = state.Using<IFactoryController>().CreateObject<TileMap>().SetPos(new Vector2(-500, -500));
            map.SetFrame(new Point(324, 324));
            map.SetScale(0.2f);
            var sheet = state.Using<IFactoryController>().CreateAsset<TileSheet>("level1");
            map.UseSheet(sheet);
            var level = state.Using<IFactoryController>().CreateAsset<LevelMap>("level1_map");
            map.UseMap(level.Map);
            state.Using<IFactoryController>().AddToState(map);
            state.Using<SurfaceMapProvider>().AddMap("level", map);

            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");
            var enemy1 = state.Using<IFactoryController>()
                .CreateObject<NaughtyShell>()
                .SetPos(new Vector2(850, 700))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddToState(state);
            enemy1.InvokeEach<Physics<NaughtyShell>>(it => it.SurfaceMap = surfaces);

            var enemy2 = state.Using<IFactoryController>()
                .CreateObject<Ben>()
                .SetPos(new Vector2(850, 700))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddToState(state);
            enemy2.InvokeEach<Physics<Ben>>(it => it.SurfaceMap = surfaces);

            var boss1 = state.Using<IFactoryController>()
                .CreateObject<Demon>()
                .SetPos(new Vector2(850, 700))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddToState(state);
            boss1.InvokeEach<Physics<Demon>>(it => it.SurfaceMap = surfaces);

            Enemy.Add(enemy1);
            Enemy.Add(enemy2);
            Enemy.Add(boss1);
        }
        private List<IEnemy> Enemy { get; set; } = [];

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            var healthIndicator = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(100, 50))
                .UseCustomFont(state, "TestFont")
                .SetScale(4f)
                .SetText("c")
                .AddToState(state);

            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");

            var obj = state.Using<IFactoryController>().CreateObject<Hero>()
                .SetPos(new Vector2(0, 0))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddComponent(new TimerHandler())
                .AddComponent(new Playable(healthIndicator))
                .AddToState(state);

            foreach (var enemy in Enemy)
            {
                enemy.SetTarget(state, obj);
            }

            obj.InvokeEach<Physics<Hero>>(it => it.SurfaceMap = surfaces);
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
