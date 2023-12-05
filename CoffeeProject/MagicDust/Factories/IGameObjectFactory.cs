using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.BaseServices;
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
        public T CreateNode<T>() where T : IDisposableComponent;
    }

    public class GameObjectFactory : IGameObjectFactory
    {


        public T CreateNode<T>() where T : IDisposableComponent
        {
            var ctor = GetCorrectConstructor(typeof(T));
            if (ctor is null)
            {
                throw new Exception($"\"{typeof(T).Name}\" object does not provide suitable constructor.");
            }
            var serviceArgs = ctor.GetParameters().Select(it => Services.ApplicationServices.GetService(it.ParameterType));
            var finalArgs = serviceArgs.ToArray();
            var obj = (T)ctor.Invoke(finalArgs);
            return obj;
        }

        private ConstructorInfo? GetCorrectConstructor(Type type)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var args = ctor.GetParameters();
                if (!args.Any() || args.All(it => Services.ApplicationServices.GetService(it.ParameterType) is not null))
                {
                    return ctor;
                }
            }
            return null;
        }
    }
}
