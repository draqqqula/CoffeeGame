using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    public class OffsetFilter(Vector2 offset) : NodeComponent, IDisplayFilter
    {
        public Vector2 Offset { get; set; } = offset;
        public DrawingParameters ApplyFilter(DrawingParameters info)
        {
            return info with { Position = info.Position + Offset };
        }
    }
}
