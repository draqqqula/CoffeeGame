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
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Organization.BaseServices;
using MagicDustLibrary.Organization.Application;
using MagicDustLibrary.Organization.StateManagement;
using Microsoft.Extensions.DependencyInjection;

namespace MagicDustLibrary.Organization
{
    public partial class GameState : IDisposable
    {

        public IServiceCollection Services { get; }

        public GameState(MagicGameApplication app, LevelSettings settings)
        {
            Services = new StateServiceCollection();
            Services.AddSingleton(app);

            app.Services.GetService<StateConfiguration>().Configure(Services, GetProvider(), settings);
        }

        public IServiceProvider GetProvider()
        {
            return new DefaultServiceProviderFactory().CreateServiceProvider(Services);
        }

        public void Update(TimeSpan deltaTime, bool onPause)
        {
            var provider = GetProvider();
            var controller = provider.GetService<IStateController>()
                ?? throw new Exception("At least one state controller must be provided");

            foreach (var service in provider.GetServices<IUpdateService>())
            {
                service.Update(controller, deltaTime, onPause);
            }
        }

        public void Draw(GameClient mainClient, SpriteBatch batch)
        {
            foreach (var displayable in GetProvider().GetServices<IDisplayService>())
            {
                displayable.Draw(mainClient, batch);
            }
        }

        public void Dispose()
        {
            foreach (var disploable in GetProvider().GetServices<IDisposable>())
            {
                disploable.Dispose();
            }
        }
    }

    public static class GameStateExtensions
    {
        public static void AddDefaultConfigurations(this MagicGameApplication app)
        {
            var config = app.Services.GetService<StateConfiguration>();
            config.AddConfiguration(ConfigureObjectServices);
            config.AddConfiguration(ConfigureClientServices);
        }

        public static void ConfigureObjectServices(IServiceCollection services, IServiceProvider provider, LevelSettings settings)
        {
            services.AddSingleton<ComponentUpdateManager>();
            services.AddSingleton<StatePictureManager>();
            services.AddSingleton<StateLayerManager>();
            services.AddSingleton<StateFamilyManager>();
            services.AddSingleton<IGameObjectFactory, GameObjectFactory>();
        }

        private static void ConfigureClientServices(IServiceCollection services, IServiceProvider provider, LevelSettings settings)
        {
            var clientManager = new StateClientManager();
            var recieveManager = new StateConnectionRecieveManager(settings.UpdateLock);

            recieveManager.OnConnected
                += clientManager.Connect;

            services.AddSingleton(clientManager);
            services.AddSingleton(new StateConnectionRecieveManager(settings.UpdateLock));
            services.AddSingleton<StateConnectionHandleManager>();
            services.AddSingleton<ViewStorage>();
            services.AddSingleton(new CameraStorage(settings.CameraSettings));

            clientManager.Connect(provider.GetService<MagicGameApplication>().MainClient);
        }

        public static void ConfigureCommonServices(StateServiceCollection services,
            LevelSettings settings, MagicGameApplication app, string levelName)
        {
            app.Services.AddService(new StateLevelManager(app.LevelManager, levelName));
            app.Services.AddService(new StateSoundManager(app.Services.GetService<IContentStorage>()));
        }
    }
}