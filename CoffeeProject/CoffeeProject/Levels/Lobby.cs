using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    public class Lobby : GameLevel
    {
        private const int DEFAULT_PORT = 7878;
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }

        protected override void Initialize(IStateController state, LevelArgs arguments)
        {
            if (int.TryParse(arguments.Data[0], out int port))
            {
                state.OpenServer(port);
            }
            state.OpenServer(DEFAULT_PORT);
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
            throw new NotImplementedException();
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            if (!client.IsRemote)
            {
                GameState.StateServices.GetService<StateClientManager>().Disconnect(client);
                return;
            }
            var allLevels = GameState.StateServices.GetService<StateLevelManager>().ApplicationLevelManager.GetAllActive();
            foreach (var level in allLevels)
            {
                if (level.GetType().GetCustomAttribute<LobbyAttribute>() is not null)
                {
                    var stateClientManager = level.GameState.StateServices.GetService<StateClientManager>();
                    stateClientManager.Connect(client);
                }
            }
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {

        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class LobbyAttribute : Attribute
    {
    }
}
