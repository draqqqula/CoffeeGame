using MagicDustLibrary.Display;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IDisplayProvider
    {
        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer);
        public bool IsVisibleFor(GameClient client);
        public Type GetLayerType();
    }
}
