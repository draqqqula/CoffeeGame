using MagicDustLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Behaviors
{
    public abstract class Behavior<T> : GameObjectComponentBase where T : class, IMultiBehaviorComponent
    {
        protected abstract void Act(IStateController state, TimeSpan deltaTime, T parent);

        [ContactComponent]
        private void GreetMultiBehavior(T parent)
        {
            parent.OnAct += Update;
        }

        private void Update(IStateController state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            Act(state, deltaTime, parent as T);
        }
    }
}
