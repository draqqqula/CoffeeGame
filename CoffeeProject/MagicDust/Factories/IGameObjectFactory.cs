using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Factorys
{
    public interface IGameObjectFactory
    {
        public T CreateObject<T>() where T : ComponentBase;
    }

    public class ComponentFactory : IGameObjectFactory
    {
        private IServiceProviderFactory<IServiceCollection> _factory;
        private IServiceCollection _services;
        public ComponentFactory(IServiceCollection services, IServiceProviderFactory<IServiceCollection> factory)
        {
            _factory = factory;
            _services = services;
        }
        public T CreateObject<T>() where T : ComponentBase
        {
            _services.TryAddTransient<T>();
            var provider = _factory.CreateServiceProvider(_services);
            return provider.GetService<T>();
        }
    }

    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly IServiceProvider _provider;
        public T CreateObject<T>() where T : ComponentBase
        {
            var ctor = GetCorrectConstructor(typeof(T));
            if (ctor is null)
            {
                throw new Exception($"\"{typeof(T).Name}\" object does not provide suitable constructor.");
            }
            var serviceArgs = ctor.GetParameters().Select(it => _provider.GetService(it.ParameterType));
            var finalArgs = serviceArgs.ToArray();
            var obj = (T)ctor.Invoke(finalArgs);
            return obj;
        }

        private ConstructorInfo? GetCorrectConstructor(Type type)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var args = ctor.GetParameters();
                if (!args.Any() || args.All(it => _provider.GetService(it.ParameterType) is not null))
                {
                    return ctor;
                }
            }
            return null;
        }

        public GameObjectFactory(IServiceProvider provider)
        {
            _provider = provider;
        }
    }
}
