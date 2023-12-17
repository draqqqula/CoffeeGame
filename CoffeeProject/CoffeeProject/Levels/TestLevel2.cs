using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
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

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            state.Using<ISoundController>().CreateSoundInstance("ANOTHER HIM", "main").Play();
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            var obj = state.Using<IFactoryController>().CreateObject<TestType2>()
                .SetPos(new Vector2(213, 454))
                .SetPlacement(new Placement<MainLayer>())
                .AddToState(state);
            obj.Client = client;
            _mainClient = client;
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            if (_mainClient is not null && _mainClient.Controls[Control.pause])
            {
                state.Using<ILevelController>().ShutCurrent(false);
            }
        }
    }
}
