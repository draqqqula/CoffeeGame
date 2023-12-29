using MagicDustLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Behaviors
{
    public abstract class Behavior<T> : NodeComponent where T : class, IMultiBehaviorComponent
    {
        protected abstract void Act(IControllerProvider state, TimeSpan deltaTime, T parent);

        [ContactComponent]
        public void GreetMultiBehavior(IMultiBehaviorComponent parent)
        {
            parent.OnAct += Update;
        }

        private void Update(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            Act(state, deltaTime, parent as T);
        }

        public Behavior()
        {
            AddGreetingFor<IMultiBehaviorComponent>(GreetMultiBehavior);
        }
    }
}
