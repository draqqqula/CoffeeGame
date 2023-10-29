using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace MagicDustLibrary.Organization
{
    public abstract class GameLevel : ILevel
    {
        #region PAUSE
        public bool OnPause { get; private set; } = false;
        public void Pause()
        {
            OnPause = true;
        }
        public void Resume()
        {
            OnPause = false;
        }
        public void TogglePause()
        {
            OnPause = !OnPause;
        }

        #endregion


        #region CONTROL
        public void Update(TimeSpan deltaTime)
        {
            if (GameState is not null)
            {
                GameState.Update(deltaTime);
                Update(GameState.Controller, deltaTime);
            }
        }

        public void Draw(GameClient mainClient, SpriteBatch spriteBatch)
        {
            if (GameState is not null)
            {
                GameState.Draw(mainClient, spriteBatch);
            }
        }

        public void Start(MagicGameApplication app, LevelArgs arguments, string name)
        {
            var state = new GameState(app, GetDefaults(), name);
            GameState = state;
            Initialize(state.Controller, arguments);
            state.BoundCustomActions(_levelClientManager);
        }

        public void Shut()
        {
            GameState = null;
        }
        #endregion


        #region ABSTRACT
        protected abstract void Initialize(IStateController state, LevelArgs arguments);
        protected abstract void OnConnect(IStateController state, GameClient client);
        protected abstract void OnDisconnect(IStateController state, GameClient client);
        protected abstract void OnClientUpdate(IStateController state, GameClient client);
        protected abstract void Update(IStateController state, TimeSpan deltaTime);
        protected abstract LevelSettings GetDefaults();
        #endregion


        #region EXTRA
        public GameState GameState { get; set; }

        private readonly LevelClientManager _levelClientManager;
        private IEnumerable<Layer> GetInitialLayers()
        {
            foreach (var attribute in GetType().GetCustomAttributes(true))
            {
                if (attribute.GetType().GetGenericTypeDefinition() == typeof(DefinedLayerAttribute<>))
                {
                    object[] parameters = { attribute.GetType().GetField("newPriority") };
                    var layerType = attribute.GetType().GetGenericArguments()[0];
                    var ctor = layerType.GetConstructor(new[] { typeof(byte) });
                    var obj = ctor.Invoke(parameters);
                    yield return (Layer)Activator.CreateInstance(attribute.GetType().GetGenericArguments()[0]);
                }
            }
        }

        public bool HasState()
        {
            return GameState is not null;
        }
        #endregion


        #region CONSTRUCTOR
        public GameLevel()
        {
            _levelClientManager = new LevelClientManager(this);
            _levelClientManager.OnConnect += OnConnect;
            _levelClientManager.OnDisconnect += OnDisconnect;
            _levelClientManager.OnUpdate += OnClientUpdate;
        }
        #endregion
    }

    public interface ILevel
    {
        public void Update(TimeSpan deltaTime);
        public void Draw(GameClient mainClient, SpriteBatch spriteBatch);
        public void Start(MagicGameApplication world, LevelArgs arguments, string name);
        public void Shut();
        public void Pause();
        public void Resume();
        public void TogglePause();
        public bool HasState();
    }
}
