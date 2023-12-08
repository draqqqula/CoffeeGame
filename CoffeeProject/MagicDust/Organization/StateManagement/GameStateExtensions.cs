using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.StateManagement
{
    public static class GameStateExtensions
    {
        public static void ConfigureDefualtServices(IServiceCollection services, LevelSettings settings)
        {
            var updateManager = new StateUpdateManager();
            var layerManager = new StateLayerManager();
            services.AddSingleton<IUpdateService>(updateManager);
            services.AddSingleton<IComponentHandler>(updateManager);
            services.AddSingleton<IComponentHandler>(layerManager);
            services.AddSingleton<StateLayerManager>(layerManager);
            services.AddSingleton<IUpdateService, StatePictureManager>();
            services.AddSingleton<IComponentHandler, StateFamilyManager>();
            services.AddSingleton<IDisplayService, DefaultDisplayService>();
            services.AddSingleton<IGameObjectFactory>(new GameObjectFactory(new DefaultServiceProviderFactory().CreateServiceProvider(services)));
        }

        public static void ConfigureNetworkServices(IServiceCollection services, LevelSettings settings)
        {
            var clientManager = new StateClientManager();
            var recieveManager = new StateConnectionRecieveManager(settings.UpdateLock);
            var cameraStorage = new CameraStorage(settings.CameraSettings);
            var viewStorage = new ViewStorage();

            services.AddSingleton(clientManager);
            services.AddSingleton(recieveManager);
            services.AddSingleton<StateConnectionHandleManager>();
            services.AddSingleton(viewStorage);
            services.AddSingleton(cameraStorage);
            services.AddSingleton<IUpdateService>(cameraStorage);

            recieveManager.OnConnected += clientManager.Connect;
            clientManager.ConfigureRelated(viewStorage);
            clientManager.ConfigureRelated(cameraStorage);


        }

        public static void ConfigureExtraServices(IServiceCollection services, LevelSettings settings)
        {
            services.AddSingleton<StateSoundManager>();
        }

        public static void AddDefualtConfigurations(this MagicGameApplication app)
        {
            app.Configurations.AddConfiguration(ConfigureNetworkServices);
            app.Configurations.AddConfiguration(ConfigureDefualtServices);
            app.Configurations.AddConfiguration(ConfigureExtraServices);
        }
    }
}
