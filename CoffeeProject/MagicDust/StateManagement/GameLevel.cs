using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using MagicDustLibrary.Organization.StateManagement;
using Microsoft.Extensions.DependencyInjection;
using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.StateClientServices;

namespace MagicDustLibrary.Organization
{
    /// <summary>
    /// Уровень игры.
    /// </summary>
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
        private readonly object _lock;

        public void Update(TimeSpan deltaTime)
        {
            lock (_lock)
            {
                if (GameState is not null)
                {
                    GameState.Update(deltaTime, OnPause);
                    Update(GameState.Controller, deltaTime);
                }
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
            var state = new GameState1();
            _defaults.AddEntry("levelName", name);
            state.ConfigureServices(app.Configurations, _defaults);

            state.ConfigureServices((services, settings) => 
            {
                services.AddSingleton(new StateLevelManager(app.LevelManager, name));
            }
            , _defaults);

            GameState = state;

            Initialize(state.Controller, arguments);
            var clientManager = state.GetProvider().GetService<StateClientManager>();
            clientManager.ConfigureRelated(_levelClientManager);
            clientManager.Connect(app.MainClient);
        }

        public void Shut()
        {
            GameState.Dispose();
            GameState = null;
        }
        #endregion


        #region ABSTRACT
        /// <summary>
        /// Вызывается при запуске уровня во время <see cref="MagicGameApplication.Launch(string)"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="client"></param>
        protected abstract void Initialize(IControllerProvider state, LevelArgs arguments);
        /// <summary>
        /// Вызывается при подключении нового игрока.<br/>
        /// В <b>однопользовательской игре</b> вызывается сразу после <see cref="Initialize"/>
        /// </summary>
        /// <param name="state"></param>
        /// <param name="client"></param>
        protected abstract void OnConnect(IControllerProvider state, GameClient client);
        /// <summary>
        /// Вызывается при отключении игрока.<br/>
        /// В <b>однопользовательской игре</b> вызывается только при отключении игрока от уровня вручную.<br/>
        /// Важно, когда игрок отключен но уровень не убран из активных в <see cref="MagicGameApplication"/>,<br/>
        /// на уровне продолжает выполняться <see cref="Update"/>. Но при этом не задействуется его отрисовка.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="client"></param>
        protected abstract void OnDisconnect(IControllerProvider state, GameClient client);
        /// <summary>
        /// Вызывается при <b>значительном изменении</b> состояния клиента<br/>
        /// Например при изменении размера окна приложения игроком.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="client"></param>
        protected abstract void OnClientUpdate(IControllerProvider state, GameClient client);
        /// <summary>
        /// Вызывается после <see cref="Game.Update(GameTime)"/>.<br/>
        /// Код, выполняемый на уровне <b>каждый кадр</b>.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="deltaTime"></param>
        protected abstract void Update(IControllerProvider state, TimeSpan deltaTime);
        /// <summary>
        /// Получение стандартных настроек для уровня.
        /// </summary>
        /// <returns></returns>
        protected abstract LevelSettings GetDefaults();
        #endregion


        #region EXTRA
        public GameState1 GameState { get; private set; }

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

        private readonly LevelSettings _defaults;
        #endregion


        #region CONSTRUCTOR
        public GameLevel()
        {
            _levelClientManager = new LevelClientManager(this);
            _levelClientManager.OnConnect += OnConnect;
            _levelClientManager.OnDisconnect += OnDisconnect;
            _levelClientManager.OnUpdate += OnClientUpdate;

            _defaults = GetDefaults();
            _lock = _defaults.UpdateLock;
        }
        #endregion
    }

    public interface ILevel
    {
        public GameState1 GameState { get; }
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
