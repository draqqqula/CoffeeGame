using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    public interface IBinding<T> where T : NodeComponent
    {
        public T Instance { get; }
    }
}
