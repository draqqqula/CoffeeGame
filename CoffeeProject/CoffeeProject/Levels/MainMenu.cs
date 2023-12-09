using CoffeeProject.GameObjects;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    internal class MainMenu : GameLevel
    {
        private GameClient _player;
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            state.Using<IFactoryController>().CreateObject<MenuImage>().SetPos(new Microsoft.Xna.Framework.Vector2(0, 0)).AddToState(state);
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            _player = client;
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            if (_player is null)
            {
                return;
            }
            if (_player.Controls.OnPress(Control.jump))
            {
                state.Using<ILevelController>().ShutCurrent(false);
            }
        }
    }
}
