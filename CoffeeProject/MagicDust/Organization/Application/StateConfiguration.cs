using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.StateManagement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.Application
{
    public class StateConfiguration
    {
        private readonly List<Action<IServiceCollection, IServiceProvider, LevelSettings>> _configurations = new();

        public void AddConfiguration(Action<IServiceCollection, IServiceProvider, LevelSettings> configuration)
        {
            _configurations.Add(configuration);
        }

        public void ClearConfigurations()
        {
            _configurations.Clear();
        }

        public void Configure(IServiceCollection services, IServiceProvider provider, LevelSettings settings)
        {
            foreach (var configuration in _configurations)
            {
                configuration(services, provider, settings);
            }
        }
    }
}
