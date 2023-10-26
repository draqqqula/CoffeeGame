using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IStateUpdateable
    {
        public void Update(IStateController state, TimeSpan deltaTime);
    }
}
