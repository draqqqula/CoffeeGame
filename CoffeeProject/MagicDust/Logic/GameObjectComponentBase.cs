using MagicDustLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public abstract class GameObjectComponentBase : ExtendedComponent, IDisposableComponent
    {
        public event OnDispose OnDisposeEvent = delegate { };

        public void Dispose()
        {
            OnDisposeEvent?.Invoke(this);
        }
    }
}
