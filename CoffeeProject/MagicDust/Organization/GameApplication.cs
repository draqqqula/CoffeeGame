using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions.Collections;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagicDustLibrary.Organization
{
    public class MagicGameApplication
    {
        public readonly GameClient MainClient;
        public readonly GameServiceContainer Services;
        private readonly Dictionary<string, ILevel> Loaded = new Dictionary<string, ILevel>();
        private readonly RelativeCollection<ILevel> ActiveLevels = new RelativeCollection<ILevel>();

        #region ACCESS

        public void PlaceTop(string name)
        {
            ActiveLevels.PlaceTop(Loaded[name]);
        }

        public void PlaceBottom(string name)
        {
            ActiveLevels.PlaceBottom(Loaded[name]);
        }

        public void PlaceAbove(string name, string source)
        {
            ActiveLevels.PlaceAbove(Loaded[name], Loaded[source]);
        }

        public void PlaceBelow(string name, string source)
        {
            ActiveLevels.PlaceBelow(Loaded[name], Loaded[source]);
        }

        #endregion

        public void Resume(string name)
        {
            ThrowWhenNotLoaded(name);
            ThrowWhenNotActive(name);
            var level = Loaded[name];
            level.Resume();
        }

        public void Pause(string name)
        {
            ThrowWhenNotLoaded(name);
            ThrowWhenNotActive(name);
            var level = Loaded[name];
            level.Pause();
        }

        public void Launch(string name)
        {
            ThrowWhenNotLoaded(name);
            if (ActiveLevels.Contains(Loaded[name]))
            {
                throw new Exception($"Level \"{name}\" already launched");
            }
            ActiveLevels.PlaceTop(Loaded[name]);
            Loaded[name].Start(this);
        }

        #region EXCEPTIONS
        private void ThrowWhenNotActive(string name)
        {
            if (!ActiveLevels.Contains(Loaded[name]))
            {
                throw new Exception($"Level \"{name}\" not loaded");
            }
        }

        private void ThrowWhenNotLoaded(string name)
        {
            if (!Loaded.ContainsKey(name) || Loaded[name] is null)
            {
                throw new Exception($"Level tagged \"{name}\" not found");
            }
        }
        #endregion

        public void LoadAs<T>(string name) where T : ILevel
        {
            Loaded[name] = Activator.CreateInstance<T>();
        }

        public void Update(TimeSpan deltaTime, GameWindow window)
        {
            if (MainClient is not null && MainClient.Window != window.ClientBounds)
            {
                MainClient.Window = window.ClientBounds;
                MainClient.OnUpdate(MainClient);
            }

            if (ActiveLevels.Length > 0)
            {
                foreach (var level in ActiveLevels.ToArray())
                    level.Update(deltaTime);
            }
        }

        public void Display(SpriteBatch batch)
        {
            if (ActiveLevels.Length > 0 && MainClient is not null)
            {
                foreach (var level in ActiveLevels.ToArray())
                    level.Draw(MainClient, batch);
            }
        }

        public MagicGameApplication(GameClient mainClient, Game game)
        {
            MainClient = mainClient;
            Services = game.Services;
            var contentStorage = new DefaultContentStorage(game.GraphicsDevice, game.Content);
            var animationProvider = new AnimationBuilder(contentStorage);
            Services.AddService(typeof(IContentStorage), contentStorage);
            Services.AddService(typeof(IAnimationProvider), animationProvider);
        }
    }
}
