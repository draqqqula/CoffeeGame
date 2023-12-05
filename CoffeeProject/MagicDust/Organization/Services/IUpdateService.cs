using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.Services
{
    public interface IUpdateService
    {
        public void Update(IStateController controller, TimeSpan deltaTime, bool onPause);
    }
}
