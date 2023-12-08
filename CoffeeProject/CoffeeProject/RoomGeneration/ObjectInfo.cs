using CoffeeProject.Encounters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Room
{
    public class RoomInfo
    {
        public Rectangle Bounds { get; private set; }
        public EncounterInfo[] Encounters { get; private set; }

        public RoomInfo(Rectangle bounds, EncounterInfo[] encounters)
        {
            Bounds = bounds;
            Encounters = encounters;
        }
    }

    public class EncounterInfo
    {
        public string Name { get; private set; }
        public Point Position { get; private set; }

        public EncounterInfo(string name, Point position)
        {
            Name = name;
            Position = position;
        }
    }

    public class TestMerge
    {
        private int Num = 0;
    }
}
