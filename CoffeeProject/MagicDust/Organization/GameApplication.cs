using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions.Collections;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagicDustLibrary.Organization
{
    /// <summary>
    /// Игровое приложение. Управляет логикой запуска, обновления и отрисовки уровней. 
    /// </summary>
    public class MagicGameApplication
    {
        public readonly GameClient MainClient;
        public readonly GameServiceContainer Services;
        public readonly ApplicationLevelManager LevelManager;

        /// <summary>
        /// Обновляет все активные уровни.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="window"></param>
        public void Update(TimeSpan deltaTime, GameWindow window)
        {
            var active = LevelManager.GetAllActive();
            if (MainClient is not null && MainClient.Window != window.ClientBounds)
            {
                MainClient.Window = window.ClientBounds;
                MainClient.OnUpdate(MainClient);
            }

            if (active.Length > 0)
            {
                foreach (var level in active)
                    level.Update(deltaTime);
            }
        }

        /// <summary>
        /// Отрисовывает все активные уровни.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch)
        {
            var active = LevelManager.GetAllActive();
            if (active.Length > 0 && MainClient is not null)
            {
                foreach (var level in active)
                    level.Draw(MainClient, batch);
            }
        }

        public MagicGameApplication(GameClient mainClient, Game game)
        {
            MainClient = mainClient;
            Services = game.Services;
            LevelManager = new ApplicationLevelManager(this);
            var contentStorage = new DefaultContentStorage(game.GraphicsDevice, game.Content);
            var animationProvider = new AnimationBuilder(contentStorage);
            Services.AddService(typeof(IContentStorage), contentStorage);
            Services.AddService(typeof(IAnimationProvider), animationProvider);
        }
    }
}
