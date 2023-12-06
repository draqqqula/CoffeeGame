using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Behaviors
{
    public interface IMultiBehaviorComponent : IDisposableComponent
    {
        public event Action<IStateController, TimeSpan, IMultiBehaviorComponent> OnAct;
    }
}
