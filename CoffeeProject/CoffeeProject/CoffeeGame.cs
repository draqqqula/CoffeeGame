using CoffeeProject.Levels;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
            _graphics.PreferMultiSampling = true;
            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1 / 60f);
            _graphics.ApplyChanges();
            Window.AllowUserResizing = true;
            Window.Title = "Farland";

            GraphicsDevice.Reset();

            var client = new GameClient(Window.ClientBounds, CreateKeyBoardControls(), GameClient.GameLanguage.Russian);
            _app = new MagicGameApplication(client, this);
            _app.LoadAs<TestLevel>("test");
            _app.Launch("test");

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _app.Display(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}