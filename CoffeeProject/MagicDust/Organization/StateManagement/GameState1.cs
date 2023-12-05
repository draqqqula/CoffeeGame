using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.StateManagement
{
    public class GameState1 : IDisposable
    {
        private readonly IServiceProviderFactory<IServiceCollection> _factory;
        private readonly IServiceCollection _services;
        private IServiceProvider GetProvider()
        {
            return _factory.CreateServiceProvider(_services);
        }

        public GameState1()
        {
            _factory = new DefaultServiceProviderFactory();
            _services = new ServiceCollection();
        }

        public void ConfigureServices(StateConfigurations configurations, LevelSettings settings)
        {
            configurations.ConfigureServices(_services, settings);
        }

        public void Update(TimeSpan deltaTime, bool onPause)
        {
            var provider = GetProvider();
            var updateables = provider.GetServices<IUpdateService>();
            var controller = provider.GetService<IStateController>() ??
                throw new Exception("at least one state controller must be registered");

            foreach (var updateable in updateables)
            {
                if (onPause && !updateable.RunOnPause)
                {
                    continue;
                }
                updateable.Update(controller, deltaTime);
            }
        }

        public void Draw(GameClient mainClient, SpriteBatch batch)
        {
            var provider = GetProvider();
            var displayables = provider.GetServices<IDisplayService>();
            
            foreach (var displayable in displayables)
            {
                displayable.Draw(mainClient, batch);
            }
        }

        public void Hook(ComponentBase component)
        {
            var provider = GetProvider();
            var handlers = provider.GetServices<IComponentHandler>();

            foreach (var handler in handlers)
            {
                handler.Hook(component);

                if (component is IDisposableComponent disposable)
                {
                    disposable.OnDisposeEvent += it => handler.Unhook((ComponentBase)it);
                }
            }
        }

        public void Dispose()
        {
            var provider = GetProvider();
            var disposables = provider.GetServices<IDisposable>();

            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
