using AsepriteImporter;
using CoffeeProject.Levels;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Network;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Data;
using CoffeeProject.SurfaceMapping;
using CoffeeProject.Collision;
using CoffeeProject.RoomGeneration;

namespace CoffeeProject
{
    public class CoffeeGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MagicGameApplication _app;

        public CoffeeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferHalfPixelOffset = true;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GraphicsDevice.SetRenderTarget(new RenderTarget2D(
                GraphicsDevice, 1920, 1080,
                false, SurfaceFormat.Color,
                DepthFormat.Depth24, 6,
                RenderTargetUsage.DiscardContents));

            _graphics.PreferredBackBufferWidth = 1903;
            _graphics.PreferredBackBufferHeight = 969;
            _graphics.PreferMultiSampling = false;
            _graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1 / 120f);
            _graphics.ApplyChanges();
            Window.AllowUserResizing = true;
            Window.Title = "Farland";
            
            GraphicsDevice.Reset();

            var client = new GameClient(Window.ClientBounds, CreateKeyBoardControls(), GameClient.GameLanguage.Russian);
            var storage = new DefaultContentStorage(_graphics.GraphicsDevice, Content);
            var parameters = new ApplicationParameters() { ContentStorage = storage, AnimationProvider = new AsepritePipelineBuilder(storage) };

            _app = new MagicGameApplication(client, parameters, this);

            _app.Services.AddService(_graphics);
            _app.Services.AddService<Game>(this);

            _app.AddDefualtConfigurations();
            _app.Configurations.AddConfiguration(SurfacesExtensions.ConfigureSurfaceHandler);
            _app.Configurations.AddConfiguration(CollisionExtensions.ConfigureCollisionHandler);
            _app.Configurations.AddConfiguration(DungeonExtensions.ConfigureDungeonController);

            _app.LevelManager.LoadAs<Dungeon1>("test");
            _app.LevelManager.LoadAs<Dungeon2>("dungeon2");
            _app.LevelManager.LoadAs<PauseMenu>("pause");
            _app.LevelManager.LoadAs<RemoteLevel>("connect");
            _app.LevelManager.LoadAs<MainMenu>("menu");
            _app.LevelManager.LoadAs<SettingsLevel>("settings");
            _app.LevelManager.LoadAs<GameOverScreen>("gameover");
            _app.LevelManager.LoadAs<NameScreen>("namescreen");
            _app.LevelManager.LoadAs<Remembrance>("memory");
            _app.LevelManager.Launch("menu", false);
            //_app.LevelManager.Launch("connect", new LevelArgs("192.168.56.101:7878"), false);

            base.Initialize();
        }
        public static GameControls CreateKeyBoardControls()
        {
            var keyboard_controls = new GameControls();
            keyboard_controls.ChangeControl(Control.left, () => Keyboard.GetState().IsKeyDown(Keys.A));
            keyboard_controls.ChangeControl(Control.right, () => Keyboard.GetState().IsKeyDown(Keys.D));
            keyboard_controls.ChangeControl(Control.jump, () => Keyboard.GetState().IsKeyDown(Keys.Space));
            keyboard_controls.ChangeControl(Control.dash, () => Keyboard.GetState().IsKeyDown(Keys.LeftShift));
            keyboard_controls.ChangeControl(Control.pause, () => Keyboard.GetState().IsKeyDown(Keys.Escape));
            keyboard_controls.ChangeControl(Control.lookUp, () => Keyboard.GetState().IsKeyDown(Keys.W));
            keyboard_controls.ChangeControl(Control.lookDown, () => Keyboard.GetState().IsKeyDown(Keys.S));
            return keyboard_controls;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            _app.Update(gameTime.ElapsedGameTime, Window);

            if (_app.LevelManager.GetAllActive().Length == 0)
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _app.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}