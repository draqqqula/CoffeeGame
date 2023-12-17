using System.Reflection;

namespace MagicDustLibrary.ComponentModel
{
    public abstract class ExtendedComponent : ComponentBase
    {
        protected ExtendedComponent()
        {
            MapConnections();
        }

        private void MapConnections()
        {
            var methods = GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ContactComponentAttribute>();
                if (attribute is null)
                {
                    continue;
                }
                var args = method.GetParameters();

                if (args.Length != 1)
                {
                    continue;
                }

                var targetType = args.First().ParameterType;
                if (!targetType.IsSubclassOf(typeof(ComponentBase)) &&
                    targetType != typeof(ComponentBase))
                {
                    continue;
                }

                var commonDelegate = AddGreetingFor<ComponentBase>;
                commonDelegate
                    .GetMethodInfo()
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(targetType)
                    .Invoke(this, new object[] { method.CreateDelegate(typeof(Action<>).MakeGenericType(targetType), this) });
            }
        }
    }
}
