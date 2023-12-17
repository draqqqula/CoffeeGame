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
using System.ComponentModel.Design;
using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.StateClientServices;

namespace MagicDustLibrary.Organization
{
    public partial class GameState : IDisposable
    {
        public GameServiceContainer ApplicationServices { get; }
        public GameServiceContainer StateServices { get; }
        public IControllerProvider Controller { get; }
        public DefaultContentStorage ContentStorage
        {
            get
            {
                return (DefaultContentStorage)ApplicationServices.GetService<IContentStorage>();
            }
        }
        private readonly LevelSettings _levelSettings;


        private void Hook(IDisposableComponent obj)
        {
        }

        private void Unhook(IDisposableComponent obj)
        {
        }

        public void Update(TimeSpan deltaTime, bool onPause)
        {
        }

        public void Draw(GameClient mainClient, SpriteBatch batch)
        {
        }

        private void ConfigureClientManagers()
        {
            StateServices.GetService<StateConnectionRecieveManager>().OnConnected
                += StateServices.GetService<StateClientManager>().Connect;
            StateServices.GetService<StateClientManager>().ConfigureRelated(StateServices.GetService<ViewStorage>());
            StateServices.GetService<StateClientManager>().ConfigureRelated(StateServices.GetService<CameraStorage>());
            StateServices.GetService<StateClientManager>().ConfigureRelated(StateServices.GetService<StateConnectionHandleManager>());
        }

        public GameState(MagicGameApplication app, LevelSettings defaults, string levelName)
        {
            ApplicationServices = app.Services;
            //Controller = new StateActions(this);
            StateServices = new GameServiceContainer();

            _levelSettings = defaults;

            AddCommonServices(StateServices, defaults, app, levelName);
            AddObjectServices(StateServices, defaults);
            AddClientServices(StateServices, defaults);

            ConfigureClientManagers();

            StateServices.GetService<StateClientManager>().Connect(app.MainClient);
        }

        private void AddCommonServices(GameServiceContainer container,
            LevelSettings settings, MagicGameApplication app, string levelName)
        {
            container.AddService(new StateLevelManager(app.LevelManager, levelName));
            container.AddService(new StateSoundManager(ApplicationServices.GetService<IContentStorage>()));
        }

        private void AddObjectServices(GameServiceContainer container, LevelSettings settings)
        {
            container.AddService(new StateUpdateManager());
            //container.AddService(new StatePictureManager());
            container.AddService(new StateLayerManager());
            //container.AddService(new StateFamilyManager());
            //container.AddService<IGameObjectFactory>(new GameObjectFactory(this));
        }

        private void AddClientServices(GameServiceContainer container, LevelSettings settings)
        {
            container.AddService(new StateClientManager());
            container.AddService(new StateConnectionRecieveManager(_levelSettings.UpdateLock));
            //container.AddService(new StateConnectionHandleManager(this));
            container.AddService(new ViewStorage());
            container.AddService(new CameraStorage(settings.CameraSettings));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}