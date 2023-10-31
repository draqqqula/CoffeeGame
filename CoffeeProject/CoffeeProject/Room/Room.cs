using CoffeeProject.Encounters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Room
{
    public class Room
    {
        public Rectangle Bounds { get; private set; }
        public bool Completed { get; private set; }
        public Encounter[] Encounters { get; private set; }

        public Room(Rectangle bounds, Encounter[] encounters)
        {
            Bounds = bounds;
            Encounters = encounters;
        }
    }
}
