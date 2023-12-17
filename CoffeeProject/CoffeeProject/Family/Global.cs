using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;

namespace CoffeeProject.Family
{
    public class Global : Family<IFamilyComponent>
    {
        protected override void CommonUpdate(IControllerProvider state, TimeSpan deltaTime)
        {
        }

        protected override void OnAbandonment(IControllerProvider state, IFamilyComponent member)
        {
        }

        protected override void OnReplenishment(IControllerProvider state, IFamilyComponent member)
        {
        }
    }
}
