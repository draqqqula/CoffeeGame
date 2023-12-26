using Assimp;
using BehaviorKit;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.ComponentModel;
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
    internal class levelilua : GameLevel
    {
        public TestVrag TestVrag { get; set; }
        public Hero Hero { get; set; }

        protected override LevelSettings GetDefaults() 
        {
            return new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            var wer = state
                .Using<IFactoryController>()
                .CreateObject<BoxDisplay.BoxDisplay>()
                .SetPlacement(new Placement<BoxDisplayLayer>())
                .UseNewTexture(new Color(0, 0, 1, 0.5f), new Color(1, 0, 0, 0.5f), 5);

            var qwerty = state.Using<IFactoryController>()
                .CreateObject<TestVrag>()
                .SetPos(new Vector2(0, 0))
                .SetBounds(new Rectangle(-25, -25, 50, 50))
                .AddComponent(wer)
                .AddToState(state);
            TestVrag = qwerty;
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            var wer = state
                .Using<IFactoryController>()
                .CreateObject<BoxDisplay.BoxDisplay>()
                .SetPlacement(new Placement<BoxDisplayLayer>())
                .UseNewTexture(new Color(0, 0, 1, 0.5f), new Color(1, 0, 0, 0.5f), 5);

            var hero = state.Using<IFactoryController>()
                .CreateObject<Hero>()
                .SetPos(new Vector2 (100, 100))
                .SetPlacement(new Placement<MainLayer>())
                .SetBounds(new Rectangle(-25, -25,50, 50))
                .AddComponent(wer)
                .AddToState(state);

            hero.Client = client;

            state.Using<IClientController>().AttachCamera(client, hero);

            Hero = hero;
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {

        }
    }
}
