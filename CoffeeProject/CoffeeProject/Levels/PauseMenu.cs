using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic.Controllers;

namespace CoffeeProject.Levels
{
    public class PauseMenu : GameLevel
    {
        private GameClient _client;
        private string PauseSource;
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            state.Using<IFactoryController>().CreateObject<PausePanel>()
                .SetPos(new Vector2(0, 0))
                .SetPlacement(Placement<CenterLayer>.On())
                .AddToState(state);
            PauseSource = arguments.Data[0];
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            _client = client;
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            if (_client.Controls.OnPress(Control.pause))
            {
                state.Using<ILevelController>().ResumeLevel(PauseSource);
                state.Using<ILevelController>().ShutCurrent(false);
            }
        }
    }
}
