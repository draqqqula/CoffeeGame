using MagicDustLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.Services
{
    public interface IComponentHandler
    {
        public void Hook(ComponentBase component);
        public void Unhook(ComponentBase component);
    }

    public abstract class ComponentHandler<T> : IComponentHandler where T : IComponent
    {
        public void Hook(ComponentBase component)
        {
            foreach (var required in component.GetComponents<T>())
            {
                Hook(required);
            }
        }

        public void Unhook(ComponentBase component)
        {
            foreach (var required in component.GetComponents<T>())
            {
                Unhook(required);
            }
        }

        public abstract void Hook(T component);
        public abstract void Unhook(T component);
    }
}
