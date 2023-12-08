using CoffeeProject.GameObjects;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
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

        protected override void Initialize(IStateController state, LevelArgs arguments)
        {
            state.CreateObject<MenuImage>().SetPos(new Microsoft.Xna.Framework.Vector2(0, 0)).AddToState(state);
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            _player = client;
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {
        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
            if (_player is null)
            {
                return;
            }
            if (_player.Controls.OnPress(Control.jump))
            {
                state.ShutCurrent(false);
            }
        }
    }
}
