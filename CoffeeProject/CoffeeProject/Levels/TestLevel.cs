using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.RoomGeneration;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.TileMap;
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
            return new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
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

            var generator = new LevelGenerator(state);
            generator.GenerateLevelGraph("TestLevel", 3, 2, 1);
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            var obj = state.Using<IFactoryController>().CreateObject<Hero>()
                .SetPos(new Vector2(0, 0))
                .SetPlacement(Placement<MainLayer>.On())
                .AddToState(state);
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
