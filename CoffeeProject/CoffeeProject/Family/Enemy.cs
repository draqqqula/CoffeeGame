using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Family
{
    public class Enemy : Family<IBodyComponent>
    {
        protected override void CommonUpdate(IControllerProvider state, TimeSpan deltaTime)
        {
        }

        protected override void OnAbandonment(IControllerProvider state, IBodyComponent member)
        {
        }

        protected override void OnReplenishment(IControllerProvider state, IBodyComponent member)
        {
        }
    }
}
