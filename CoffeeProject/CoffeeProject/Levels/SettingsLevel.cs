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
    internal class SettingsLevel : GameLevel
    {
        private GameClient _player;
        private List<Label> _labels = [];
        private int _selectedIndex;
        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings();
        }


        class VSyncSetting : NodeComponent
        {
            private readonly GraphicsDeviceManager _graphics;
            private readonly Game _game;
            public VSyncSetting(MagicGameApplication app)
            {
                _graphics = app.Services.GetService<GraphicsDeviceManager>();
                _game = app.Services.GetService<Game>();
            }

            public bool Setting
            {
                get
                {
                    return _graphics.SynchronizeWithVerticalRetrace;
                }
                set
                {
                    _graphics.SynchronizeWithVerticalRetrace = value;
                    if (value)
                    {
                        _game.IsFixedTimeStep = false;
                    }
                    else
                    {
                        _game.IsFixedTimeStep = true;
                    }
                    _graphics.ApplyChanges();
                }
            }

            public void Toggle()
            {
                Setting = !Setting;
            }
        }

        class FPSSetting : NodeComponent
        {
            private readonly GraphicsDeviceManager _graphics;
            private readonly Game _game;
            private readonly double[] _options = [30, 60, 70, 120, 240, 360];
            private int index = 4;
            public FPSSetting(MagicGameApplication app)
            {
                _graphics = app.Services.GetService<GraphicsDeviceManager>();
                _game = app.Services.GetService<Game>();
            }

            public double Setting => _options[index];

            public void Scroll()
            {
                if (index < _options.Length - 1)
                {
                    index += 1;
                }
                else
                {
                    index = 0;
                }
                _game.TargetElapsedTime = TimeSpan.FromSeconds(1/Setting);
                _graphics.ApplyChanges();
            }
        }


        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            var vsync = state.Using<IFactoryController>()
                .CreateObject<VSyncSetting>();
            var fps = state.Using<IFactoryController>()
                .CreateObject<FPSSetting>();

            var startGame = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText($"V-sync: {vsync.Setting}")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(700, 400))
                .AddComponent(new ButtonAction(() =>
                {
                    vsync.Toggle();
                    _labels[0].SetText($"V-sync: {vsync.Setting}");
                }))
                .AddToState(state);

            var settings = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText($"Частота кадров: {fps.Setting}")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(700, 550))
                .AddComponent(new ButtonAction(() =>
                {
                    fps.Scroll();
                    _labels[1].SetText($"Частота кадров: {fps.Setting}");
                }))
                .AddToState(state);

            var quit = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .UseFont(state, "Caveat")
                .SetText("назад")
                .SetScale(1f)
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(700, 700))
                .AddComponent(new ButtonAction(() =>
                {
                    state.Using<ILevelController>().ResumeLevel("menu");
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
