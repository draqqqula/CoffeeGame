using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    internal interface IEnemy
    {
        public void SetTarget(IControllerProvider state, GameObject target);
    }
}
