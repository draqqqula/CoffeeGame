using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.StateManagement.DefualtImplementations
{
    public class StateHooker
    {
        private IServiceProvider _provider;
        public StateHooker(IServiceProvider provider)
        {
            _provider = provider;
        }
        public void Hook(ComponentBase component)
        {
            var handlers = _provider.GetServices<IComponentHandler>();

            foreach (var handler in handlers)
            {
                handler.Hook(component);

                if (component is IDisposableComponent disposable)
                {
                    disposable.OnDisposeEvent += it => handler.Unhook((ComponentBase)it);
                }
            }
        }
    }
}
