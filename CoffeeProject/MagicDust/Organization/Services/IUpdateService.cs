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
        public bool RunOnPause { get; }

        public void Update(IStateController controller, TimeSpan deltaTime);
    }
}
