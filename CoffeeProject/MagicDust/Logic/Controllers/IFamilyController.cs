using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Organization.DefualtImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Controllers
{
    public interface IFamilyController : IStateController
    {
        public F GetFamily<F>() where F : class, IFamily;
    }

    internal class DefaultFamilyController : IFamilyController
    {
        private readonly StateFamilyManager _familyManager;
        public DefaultFamilyController(StateFamilyManager familyManager)
        {
            _familyManager = familyManager;
        }

        public F GetFamily<F>() where F : class, IFamily
        {
           return  _familyManager.GetFamily<F>();
        }
    }
}
