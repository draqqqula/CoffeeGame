using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Point = Microsoft.Xna.Framework.Point;

namespace CoffeeProject.RoomGeneration
{
    public class LevelInfo
    {
        public string Name { get; }
        public RoomInfo[] EnemyRooms { get; }
        public RoomInfo[] LootRooms { get; }
        public RoomInfo StartRoom { get; }
        public RoomInfo BossRoom { get; }
    }

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
}
