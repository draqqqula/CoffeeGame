using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public record class LevelSettings
    {
        public CameraSettings CameraSettings { get; init; }
    }
}
