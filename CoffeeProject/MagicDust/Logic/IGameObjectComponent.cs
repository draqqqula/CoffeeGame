using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public delegate void OnDispose(IGameObjectComponent target);

namespace MagicDustLibrary.Logic
{
    public interface IGameObjectComponent : IDisposable, IComponent
    {
        public event OnDispose OnDisposeEvent;
    }
}
