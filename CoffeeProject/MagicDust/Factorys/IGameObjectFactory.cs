using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
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
        public T CreateObject<T>() where T : GameObject;
    }

    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly GameState _state;
        public T CreateObject<T>() where T : GameObject
        {
            var ctor = GetCorrectConstructor(typeof(T));
            if (ctor is null)
            {
                throw new Exception($"\"{typeof(T).Name}\" object does not provide suitable constructor.");
            }
            var serviceArgs = ctor.GetParameters().Select(it => _state.ApplicationServices.GetService(it.ParameterType));
            var finalArgs = serviceArgs.ToArray();
            var obj = (T)ctor.Invoke(finalArgs);
            return obj;
        }

        private ConstructorInfo? GetCorrectConstructor(Type type)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var args = ctor.GetParameters();
                if (!args.Any() || args.All(it => _state.ApplicationServices.GetService(it.ParameterType) is not null))
                {
                    return ctor;
                }
            }
            return null;
        }

        public GameObjectFactory(GameState state)
        {
            _state = state;
        }
    }
}
