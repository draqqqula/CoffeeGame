using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    public interface IDungeonLevel
    {
        public int Level { get; }
        public Vector2 PlayerPosition { get; set; }
    }
}
