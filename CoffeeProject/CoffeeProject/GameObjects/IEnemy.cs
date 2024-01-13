using CoffeeProject.Family;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [MemberShip<Enemy>]
    public interface IEnemy : IFamilyComponent
    {
        public void SetTarget(IControllerProvider state, GameObject target);
    }
}
