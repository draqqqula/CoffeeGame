using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Logic;
using GraphShape;
using MagicDustLibrary.Logic.Behaviors;

namespace BehaviorKit
{
    public class ShapeCollider : Behavior<IMultiBehaviorComponent>
    {

        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
        }
    }
}
