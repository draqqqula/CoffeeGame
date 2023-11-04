using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Network
{
    public interface IUnpacker
    {
        public void Unpack(byte[] data);
    }
}
