using MagicDustLibrary.Logic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.StateManagement
{
    public delegate void ConfigurationDelegate(IServiceCollection services, LevelSettings settings);

    public class StateConfigurations
    {
        private readonly List<ConfigurationDelegate> configurations = new();
        
        public void AddConfiguration(ConfigurationDelegate configuration)
        {
            configurations.Add(configuration);
        }

        public void ConfigureServices(IServiceCollection services, LevelSettings settings)
        {
            foreach (var configuration in configurations)
            {
                configuration(services, settings);
            }
        }
    }
}
