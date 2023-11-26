using CoffeeProject.Encounters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Point = Microsoft.Xna.Framework.Point;
using System.IO;

namespace CoffeeProject.Room
{
    public class RoomAbstraction
    {
        public int RoomNumber;

        public RoomAbstraction(int roomNumber)
        {
            RoomNumber = roomNumber;
        }
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

    public class LevelGenerator
    {
        //private static int NumberOfRooms = 10;

        public static Level GenerateLevel(int levelNumber, int numberOfRooms)
        {
            var rnd = new Random();
            var numberOfRoomTypes = new DirectoryInfo($@"\CoffeeGame\CoffeeProject\CoffeeProject\Content\Levels\Rooms_Level{levelNumber}").GetFiles().Length / 4;

            var level = new Level(numberOfRooms);

            // Добавление начальной и конечной комнат
            level[0].AddRoomInfo(levelNumber, 0);
            level[numberOfRooms - 1].AddRoomInfo(levelNumber, 1);

            // Добавление промежуточных комнат
            for (int i = 1; i < numberOfRooms - 2; i++)
            {
                var roomType = rnd.Next(2, numberOfRoomTypes);
                level[i].AddRoomInfo(levelNumber, roomType);
            }

            return level;
        }
    }

    public class Room
    {
        // Это Node в Graph

        private readonly List<Room> connectedRooms = new List<Room>();
        public readonly int RoomNumber;

        public RoomInfo RoomInfo;

        public Room(int number)
        {
            RoomNumber = number;
        }

        public IEnumerable<Room> ConnectedRooms
        {
            get
            {
                foreach (var room in connectedRooms)
                    yield return room;
            }
        }
        public static void Connect(Room node1, Room node2, Level graph)
        {
            if (!graph.Rooms.Contains(node1) || !graph.Rooms.Contains(node2)) throw new ArgumentException();
            node1.connectedRooms.Add(node2);
            node2.connectedRooms.Add(node1);
        }

        public void AddRoomInfo(int levelNumber, int roomType)
        {

        }
        //public void AddRoomInfo(RoomInfo roomInfo)
        //{
        //    this.RoomInfo = roomInfo;
        //}
    }

    public class Level
    {
        // Это Graph

        //private int LevelNumber;
        private Room[] rooms;
        public Level(int nodesCount)
        {
            //this.LevelNumber = levelNumber;
            rooms = Enumerable.Range(0, nodesCount).Select(z => new Room(z)).ToArray();
        }

        public int Length { get { return rooms.Length; } }

        public Room this[int index] { get { return rooms[index]; } }

        public IEnumerable<Room> Rooms
        {
            get
            {
                foreach (var room in rooms) 
                    yield return room;
            }
        }

        public void Connect(int index1, int index2)
        {
            Room.Connect(rooms[index1], rooms[index2], this);
        }
    }
}
