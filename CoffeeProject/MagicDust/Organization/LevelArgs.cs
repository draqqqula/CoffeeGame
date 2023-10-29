using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    /// <summary>
    /// Параметры, передаваемые при запуске уровня.
    /// </summary>
    public class LevelArgs
    {
        string[] Data;
        public static LevelArgs Empty
        {
            get
            {
                return new LevelArgs();
            }
        }

        public LevelArgs(params string[] data)
        {
            Data = data;
        }
    }
}
