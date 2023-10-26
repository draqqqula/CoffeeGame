using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    internal interface IMultiBehavior
    {
        public void AddBehavior(string name, IBehavior behavior);

        public T GetBehavior<T>(string name) where T : IBehavior;

        public void AddBehavior(IBehavior behavior)
        {
            AddBehavior(behavior.GetType().Name, behavior);
        }

        public void AddBehaviors(params IBehavior[] behaviors)
        {
            foreach (var behavior in behaviors)
            {
                AddBehavior(behavior);
            }
        }

        public T GetBehavior<T>() where T : IBehavior
        {
            return GetBehavior<T>(typeof(T).Name);
        }
    }
}
