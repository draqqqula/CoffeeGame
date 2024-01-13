using CoffeeProject.GameObjects;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Weapons
{
    public interface IPlayerAbility
    {
        public void UseAbility(IControllerProvider state, Hero player);
    }
}
