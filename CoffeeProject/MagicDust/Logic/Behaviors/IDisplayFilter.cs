using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic.Behaviors
{
    public interface IDisplayFilter : IComponent
    {
        public DrawingParameters ApplyFilter(DrawingParameters info);
    }
}
