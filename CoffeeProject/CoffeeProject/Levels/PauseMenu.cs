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

        protected override void Initialize(IStateController state, LevelArgs arguments)
        {
            state.CreateObject<PauseTitle, MainLayer>(new Vector2(500, 500));
            PauseSource = arguments.Data[0];
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            _client = client;
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {
        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
            if (_client.Controls.OnPress(Control.pause))
            {
                state.ResumeLevel(PauseSource);
                state.ShutCurrent(false);
            }
        }
    }
}
