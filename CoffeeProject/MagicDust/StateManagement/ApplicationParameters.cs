using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class ApplicationParameters
    {
        public IAnimationProvider? AnimationProvider { get; init; }
        public IContentStorage? ContentStorage { get; init; }
    }
}
