using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Family
{
    internal class TimeStopImmune : Family<IUpdateComponent>
    {
        protected override void CommonUpdate(IControllerProvider state, TimeSpan deltaTime)
        {
        }

        protected override void OnAbandonment(IControllerProvider state, IUpdateComponent member)
        {
        }

        protected override void OnReplenishment(IControllerProvider state, IUpdateComponent member)
        {
        }
    }
}
