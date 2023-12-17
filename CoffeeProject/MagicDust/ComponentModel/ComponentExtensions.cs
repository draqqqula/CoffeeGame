using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.ComponentModel
{
    public static class ComponentExtensions
    {
        public static void InvokeEach<T>(this ComponentBase component, Action<T> action)
        {
            foreach (var item in component.GetComponents<T>())
            {
                action(item);
            }
        }
    }
}
