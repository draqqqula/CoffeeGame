using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Net.Sockets;
using System.Net;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Display;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Content;
using Microsoft.Xna.Framework.Graphics;
using MagicDustLibrary.Factorys;

namespace MagicDustLibrary.Organization
{
    public partial class GameState
    {
        public GameServiceContainer Services { get; }
        public IStateController Controller { get; }
        public DefaultContentStorage ContentStorage
        {
            get
            {
                return (DefaultContentStorage)Services.GetService<IContentStorage>();
            }
        }

        //object related
        private StateUpdateManager _stateUpdateManager = new();
        private StatePictureManager _statePictureManager = new();
        private StateLayerManager _stateLayerManager = new();
        private StateFamilyManager _stateFamilyManager = new();
        private IGameObjectFactory _gameObjectFactory;

        //client related
        private LevelSettings _levelSettings;
        private StateClientManager _stateClientManager = new();
        private StateConnectionRecieveManager _stateConnectionManager = new();
        private ViewStorage _viewStorage = new();
        private CameraStorage _cameraStorage;

        private StateLevelManager _stateLevelManager;

        private void Hook(GameObject obj)
        {
            _stateUpdateManager.AddUpdateable(obj);
            _stateLayerManager.GetLayer(obj.GetLayerType()).PlaceTop(obj);

            obj.OnDisposed += Unhook;

        }

        private void Unhook(GameObject obj)
        {
            _stateUpdateManager.RemoveUpdateable(obj);
            _stateLayerManager.GetLayer(obj.GetLayerType()).Remove(obj);
        }

        public void Update(TimeSpan deltaTime)
        {
            _stateUpdateManager.Update(Controller, deltaTime);

            _statePictureManager.UpdatePicture(
                _stateLayerManager.GetAll(),
                _stateClientManager.GetAll(),
                _cameraStorage,
                _viewStorage);
        }

        public void Draw(GameClient mainClient, SpriteBatch batch)
        {
            foreach (var displayable in _viewStorage.GetFor(mainClient).GetAndClear())
            {
                displayable.Draw(batch, _cameraStorage.GetFor(mainClient), Services.GetService<IContentStorage>());
            }
        }

        private void ConfigureManagers()
        {
            _stateConnectionManager.OnConnected += _stateClientManager.Connect;
            _stateClientManager.ConfigureRelated(_viewStorage);
            _stateClientManager.ConfigureRelated(_cameraStorage);
        }

        public void BoundCustomActions(ClientRelatedActions customActions)
        {
            _stateClientManager.ConfigureRelated(customActions);
        }

        public GameState(MagicGameApplication app, LevelSettings defaults)
        public GameState(MagicGameApplication app, LevelSettings defaults, string levelName)
        {
            Services = app.Services;
            _levelSettings = defaults;
            _cameraStorage = new CameraStorage(defaults.CameraSettings);
            _gameObjectFactory = new GameObjectFactory(this);
            _stateLevelManager = new StateLevelManager(app.LevelManager, levelName);
            Controller = new StateActions(this);
            ConfigureManagers();
            _stateClientManager.Connect(app.MainClient);
        }
    }
}
