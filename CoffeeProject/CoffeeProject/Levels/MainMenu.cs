﻿using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.ComponentModel;
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
    internal class MainMenu : GameLevel
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
            state.Using<IFactoryController>()
                .CreateObject<MenuImage>()
                .SetPos(new Vector2(0, 0))
                .AddToState(state);

            var startGame = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText("начать")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(300, 400))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().ShutCurrent(false);
                    state.Using<ILevelController>().LaunchLevel("namescreen", false);
                }))
                .AddToState(state);

            var settings = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText("настройки")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(300, 550))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().PauseCurrent();
                    state.Using<ILevelController>().LaunchLevel("settings", false);
                }))
                .AddToState(state);

            var quit = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText("выйти")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(300, 700))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().ShutCurrent(false);
                }))
                .AddToState(state);

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

    class ButtonAction : NodeComponent
    {
        private readonly Action _action;
        public ButtonAction(Action action)
        {
            _action = action;
        }

        public void Invoke()
        {
            _action();
        }
    }
}
