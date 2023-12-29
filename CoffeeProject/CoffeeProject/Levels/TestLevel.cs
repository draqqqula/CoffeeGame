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
            Enemy = state.Using<IFactoryController>()
                .CreateObject<NaughtyShell>()
                .SetPos(new Vector2(850, 700))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddToState(state);
            Enemy.InvokeEach<Physics<NaughtyShell>>(it => it.SurfaceMap = surfaces);
        }
        private NaughtyShell Enemy { get; set; }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");

            var obj = state.Using<IFactoryController>().CreateObject<Hero>()
                .SetPos(new Vector2(0, 0))
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddHealthLabel(state)
                .AddToState(state);

            Enemy.SetTarget(obj);

            obj.InvokeEach<Physics<Hero>>(it => it.SurfaceMap = surfaces);
            var dummy = obj.GetComponents<Dummy>().First();

            obj.Client = client;

            state.Using<IClientController>().AttachCamera(client, obj);
            state.Using<IFactoryController>().CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(50, 50)).AddToState(state);
            state.Using<IFactoryController>().CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(150, 50)).AddToState(state);
            state.Using<IFactoryController>().CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(250, 50)).AddToState(state);
            state.Using<IFactoryController>().CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(350, 50)).AddToState(state);
            state.Using<IFactoryController>().CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(450, 50)).AddToState(state);
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
        }
    }
}
