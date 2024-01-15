using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    internal class GameOverScreen : GameLevel
    {
        private GameClient _player;
        private List<Label> _labels = [];
        private int _selectedIndex;
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            var startGame = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetText("заново")
                .UseFont(state, "Caveat")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(700, 400))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().RestartLevel("test", arguments);
                    state.Using<ILevelController>().ShutCurrent(false);
                }))
                .AddToState(state);

            var settings = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetText("в меню")
                .UseFont(state, "Caveat")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(700, 550))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().ShutLevel("test", false);
                    state.Using<ILevelController>().LaunchLevel("menu", false);
                    state.Using<ILevelController>().ShutCurrent(false);
                }))
                .AddToState(state);

            var quit = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText("выйти")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(700, 700))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().ShutLevel("test", false);
                    state.Using<ILevelController>().ShutCurrent(false);
                }))
                .AddToState(state);
            _labels.Clear();

            _labels.Add(startGame);
            _labels.Add(settings);
            _labels.Add(quit);

            _selectedIndex = 0;
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
                _labels[_selectedIndex].InvokeEach<ButtonAction>(it => it.Invoke());
            }
            if (_player.Controls.OnPress(Control.lookUp))
            {
                _selectedIndex = Math.Clamp(_selectedIndex - 1, 0, _labels.Count - 1);
            }
            if (_player.Controls.OnPress(Control.lookDown))
            {
                _selectedIndex = Math.Clamp(_selectedIndex + 1, 0, _labels.Count - 1);
            }
            foreach (var label in _labels)
            {
                label.Color = Color.White;
            }
            _labels[_selectedIndex].Color = Color.Yellow;
        }
    }
}
