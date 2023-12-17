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
        public void GetFamily<F>() where F : class, IFamily;
    }

    internal class DefaultFamilyController : IFamilyController
    {
        private readonly StateFamilyManager _familyManager;
        public DefaultFamilyController(StateFamilyManager familyManager)
        {
            _familyManager = familyManager;
        }

        public void GetFamily<F>() where F : class, IFamily
        {
            _familyManager.GetFamily<F>();
        }
    }
}
