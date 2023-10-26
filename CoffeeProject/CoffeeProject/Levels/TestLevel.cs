using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override void Initialize(IStateController state)
        {
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            var obj = state.CreateObject<TestType, MainLayer>(new Vector2(500, 500));
            obj.Client = client;
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {
        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
        }
    }
}
