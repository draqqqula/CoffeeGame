using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Extensions
{
    public class RandomEx : Random
    {
        public float NextSingle(float min, float max, float factor)
        {
            return min + (MathF.Pow(NextSingle(), factor) * (max - min));
        }
    }
}
