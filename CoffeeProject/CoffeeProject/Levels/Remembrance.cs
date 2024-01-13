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
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            state.Using<ILevelController>().ShutLevel("test", false);
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {

        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {

        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {

        }
    }
}
