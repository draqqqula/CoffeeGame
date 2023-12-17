using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.StateClientServices;
using MagicDustLibrary.Organization.StateManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MagicDustLibrary.Logic.Controllers
{
    class DefaultStateController : IControllerProvider
    {
        private readonly GameState1 _state;
        public DefaultStateController(GameState1 state)
        {
            _state = state;
        }
        public T Using<T>() where T : IStateController
        {
            var controller = _state.GetProvider().GetService<T>();
            if (controller is null)
            {
                throw new Exception($"{typeof(T).Name} not found");
            }
            return controller;
        }
    }
}