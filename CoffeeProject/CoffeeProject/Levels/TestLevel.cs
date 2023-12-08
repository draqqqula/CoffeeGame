using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.TileMap;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
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

        protected override void Initialize(IStateController state, LevelArgs arguments)
        {
            //state.OpenServer(7878);
            var map = state.CreateObject<TileMap>().SetPos(new Vector2(-500, -500));
            map.SetFrame(new Point(324, 324));
            map.SetScale(0.2f);
            var sheet = state.CreateAsset<TileSheet>("level1");
            map.UseSheet(sheet);
            var level = state.CreateAsset<LevelMap>("level1_map");
            map.UseMap(level.Map);
            state.AddToState(map);
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            var obj = state.CreateObject<Hero>()
                .SetPos(new Vector2(0, 0))
                .SetPlacement(Placement<MainLayer>.On())
                .AddToState(state);
            obj.Client = client;
            state.AttachCamera(client, obj);
            state.CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(50, 50)).AddToState(state);
            state.CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(150, 50)).AddToState(state);
            state.CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(250, 50)).AddToState(state);
            state.CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(350, 50)).AddToState(state);
            state.CreateObject<Heart>().SetPlacement(new Placement<GUI>()).SetPos(new Vector2(450, 50)).AddToState(state);
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {
        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
            int a = 1;
        }
    }
}
