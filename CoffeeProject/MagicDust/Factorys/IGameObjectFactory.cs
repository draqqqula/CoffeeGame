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
        public T CreateObject<T, L>(Vector2 position) where T : GameObject where L : Layer;
    }

    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly GameState _state;
        public T CreateObject<T, L>(Vector2 position) where T : GameObject where L : Layer
        {
            var ctor = GetCorrectConstructor(typeof(T));
            if (ctor is null)
            {
                throw new Exception($"\"{typeof(T).Name}\" object does not provide suitable constructor.");
            }
            var placement = new Placement<L>();
            var defaultArgs = new object[] { placement, position };
            var serviceArgs = ctor.GetParameters().Skip(2).Select(it => _state.ApplicationServices.GetService(it.ParameterType));
            var finalArgs = defaultArgs.Concat(serviceArgs).ToArray();
            var obj = (T)ctor.Invoke(finalArgs);
            return obj;
        }

        private ConstructorInfo? GetCorrectConstructor(Type type)
        {
            foreach (var ctor in type.GetConstructors())
            {
                var args = ctor.GetParameters();
                if (args[0].Name == "placement" && args[0].ParameterType == typeof(IPlacement) &&
                    args[1].Name == "position" && args[1].ParameterType == typeof(Vector2))
                {
                    var serviceArgs = args.Skip(2);
                    if (!serviceArgs.Any() || serviceArgs.All(it => _state.ApplicationServices.GetService(it.ParameterType) is not null))
                    {
                        return ctor;
                    }
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
