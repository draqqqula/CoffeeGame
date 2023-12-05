using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Organization.StateManagement;

namespace MagicDustLibrary.Organization.BaseServices
{
    public interface IUpdateService
    {
        public void Update(IStateController controller, TimeSpan deltaTime, bool onPause);
    }
}
