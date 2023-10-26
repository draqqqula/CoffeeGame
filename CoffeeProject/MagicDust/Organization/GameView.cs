using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class GameView
    {
        private readonly List<IDisplayable> _displayables = new List<IDisplayable>();

        public void Add(IDisplayable displayable)
        {
            _displayables.Add(displayable);
        }

        public IEnumerable<IDisplayable> GetAndClear()
        {
            var buffer = _displayables.ToArray();
            _displayables.Clear();
            return buffer;
        }
    }
}
