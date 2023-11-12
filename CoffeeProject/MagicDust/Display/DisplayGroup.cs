using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Display
{
    public class DisplayGroup
    {
    }

    public interface IDisplayGroup
    {
        public string Name { get; }
        public void AddClient(GameClient client);
        public void RemoveClient(GameClient client);

    }
}
