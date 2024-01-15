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
    public class Remembrance : GameLevel
    {
        private LevelArgs Arguments { get; set; }
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            state.Using<ILevelController>().ShutLevel("test", false);
            Arguments = arguments;
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            state.Using<ILevelController>().LaunchLevel("dungeon2", Arguments, false);
            state.Using<ILevelController>().ShutCurrent(false);
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {

        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {

        }
    }
}
