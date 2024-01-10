using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Point = Microsoft.Xna.Framework.Point;
using System.Dynamic;
using MagicDustLibrary.Content;
using Microsoft.Xna.Framework.Graphics;
using MagicDustLibrary.Display;

namespace CoffeeProject.RoomGeneration
{
    public class LevelInfo
    {
        public LevelInfo(
            [FromStorage("Levels", "*", "level")] ExpandoObject levelFile,
            [FromInput(0)]string levelName,
            IContentStorage content
            )
        {
            CreateRoomInfos(levelFile, content, levelName);
            Name = levelName;
        }

        private void CreateRoomInfos(dynamic levelFile, IContentStorage content, string levelName)
        {
            string startRoomFile = levelFile.StartRoom;
            string bossRoomFile = levelFile.BossRoom;
            List<object> enemyRoomFiles = levelFile.EnemyRooms;
            List<object> lootRoomFiles = levelFile.LootRooms;
            StartRoom = AssetExtensions.Create<RoomInfo>(content, levelName, startRoomFile);
            BossRoom = AssetExtensions.Create<RoomInfo>(content, levelName, bossRoomFile);

            var enemyRooms = new List<RoomInfo>();
            foreach ( object roomFile in enemyRoomFiles )
            {
                string roomName = roomFile.ToString();
                var room = AssetExtensions.Create<RoomInfo>(content, levelName, roomName);
                enemyRooms.Add(room);
            }
            EnemyRooms = enemyRooms.ToArray();

            var lootRooms = new List<RoomInfo>();
            foreach (object roomFile in lootRoomFiles)
            {
                string roomName = roomFile.ToString();
                var room = AssetExtensions.Create<RoomInfo>(content, levelName, roomName);
                lootRooms.Add(room);
            }
            LootRooms = lootRooms.ToArray();
        }

        public string Name { get; private set; }
        public RoomInfo[] EnemyRooms { get; private set; }
        public RoomInfo[] LootRooms { get; private set; }
        public RoomInfo StartRoom { get; private set; }
        public RoomInfo BossRoom { get; private set; }
    }

    public class RoomInfo
    {
        public RoomInfo(
            [FromStorage("Levels", "*", "*", "info")] ExpandoObject info,
            [FromStorage("Levels", "*", "*", "encounters")] List<ExpandoObject> encounters,
            [FromStorage("Levels", "*", "*", "background")] Texture2D background,
            [FromStorage("Levels", "*", "*", "tilemap")] Texture2D tilemap
        )
        {
            Background = background;
            TileMap = tilemap;
            FillEncounters(encounters);
            FillBounds(info);
            FillGates(info);
        }
        public Point Bounds { get; private set; }
        public EncounterInfo[] Encounters { get; private set; }
        public Texture2D Background { get; private set; }
        public Texture2D TileMap { get; private set; }
        public Point[] Gates { get; private set; }

        private EncounterInfo ParseEncounter(dynamic obj)
        {
            string name = obj.name;
            List<object> position = obj.position;
            long positionX = (long)position[0];
            long positionY = (long)position[1];
            var point = new Point(Convert.ToInt32(positionX), Convert.ToInt32(positionY));
            return new EncounterInfo(name, point);
        }

        private void FillEncounters(dynamic file)
        {
            var result = new List<EncounterInfo>();
            foreach (dynamic obj in file)
            {
                result.Add(ParseEncounter(obj));
            }
            Encounters = result.ToArray();
        }

        private void FillBounds(dynamic file)
        {
            Bounds = ToPoint(file.bounds);
        }

        private Point ToPoint(dynamic arr)
        {
            long boundsWidth = arr[0];
            long boundsHeight = arr[1];
            return new Point(Convert.ToInt32(boundsWidth), Convert.ToInt32(boundsHeight));
        }

        private void FillGates(dynamic file)
        {
            var result = new List<Point>();
            List<object> points = file.gates;
            foreach (dynamic point in points)
            {
                result.Add(ToPoint(point));
            }
            Gates = result.ToArray();
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
