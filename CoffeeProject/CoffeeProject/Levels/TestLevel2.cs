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
    public class TestLevel2 : GameLevel
    {
        GameClient _mainClient;
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings
            {
                CameraSettings = new CameraSettings(),
            };
        }

        protected override void Initialize(IStateController state, LevelArgs arguments)
        {
            state.CreateSoundInstance("ANOTHER HIM", "main").Play();
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            var obj = state.CreateObject<TestType2, TestLayer>(new Vector2(500, 800));
            obj.Client = client;
            _mainClient = client;
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {
        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
            if (_mainClient is not null && _mainClient.Controls[Control.pause])
            {
                state.ShutCurrent(false);
            }
        }
    }
}
