using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Organization.BaseServices;

namespace MagicDustLibrary.Logic
{
    public interface IMultiBehaviorComponent : IDisposableComponent
    {
        public event Action<IStateController, TimeSpan, IMultiBehaviorComponent> OnAct;
    }
}
