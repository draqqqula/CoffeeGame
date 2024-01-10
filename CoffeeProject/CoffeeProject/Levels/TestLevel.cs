using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.BoxDisplay;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.Run;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.Image;
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    public class TestLevel : GameLevel
    {
        private Image Vignette { get; set; }
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
            state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Music", "dungeon"), "sound").Play();
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
                .AddShadow(state)
                .AddToState(state);
            enemy1.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);

            var enemy2 = state.Using<IFactoryController>()
                .CreateObject<Ben>()
                .SetPos(new Vector2(850, 700))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddShadow(state)
                .AddToState(state);
            enemy2.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);

            var boss1 = state.Using<IFactoryController>()
                .CreateObject<BossSpawner>()
                .SetPos(new Vector2(850, 700))
                .SetBounds(new Rectangle(-20, -20, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddShadow(state)
                .AddToState(state);
            boss1.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);

            Vignette = state.Using<IFactoryController>().CreateObject<Image>()
                .SetPlacement(new Placement<TintLayer>())
                .SetTexture("vignette")
                .AddToState(state);

            Enemy.Add(enemy1);
            Enemy.Add(enemy2);
        }
        private List<IEnemy> Enemy { get; set; } = [];

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
            Vignette
                .SetScale(client.Window.Size.ToVector2() / Vignette.TextureBounds.Size.ToVector2())
                .SetPos(client.Window.Size.ToVector2() / 2);
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            Vignette
                .SetScale(client.Window.Size.ToVector2() / Vignette.TextureBounds.Size.ToVector2())
                .SetPos(client.Window.Size.ToVector2() / 2);

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
                .AddShadow(state)
                .AddToState(state);

            foreach (var enemy in Enemy)
            {
                enemy.SetTarget(state, obj);
            }

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