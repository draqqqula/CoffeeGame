using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Family
{
    public class Global : Family<Sprite>
    {
        protected override void CommonUpdate(IStateController state, TimeSpan deltaTime)
        {
            foreach (var obj in Members)
            {
                obj.SetPosition(obj.GetPosition() + new Vector2(0.1f, 0));
            }
        }

        protected override void OnAbandonment(IStateController state, Sprite member)
        {
        }

        protected override void OnReplenishment(IStateController state, Sprite member)
        {
        }
    }
}
