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
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;

namespace CoffeeProject.RoomGeneration
{
    public class LevelGenerator
    {
        private IControllerProvider _state;
        public LevelGenerator(IControllerProvider state)
        {
            _state = state;
        }

        public LevelGraph GenerateLevelGraph(string levelName, int mainPathRoomsCount, int enemyRoomsCount, int lootRoomsCount)
        {
            var rnd = new Random();
            var levelInfo = _state.Using<IFactoryController>().CreateAsset<LevelInfo>(levelName);
            var level = new LevelGraph(enemyRoomsCount, lootRoomsCount, mainPathRoomsCount);
            var roomsCount = 2 + enemyRoomsCount + lootRoomsCount;

            #region
            // Добавление стартовой комнаты
            level[0].AddRoomInfo(levelInfo.StartRoom);

            // Добавление комнат с противниками на главном пути
            for (int i = 1; i <= mainPathRoomsCount; i++)
            {
                var enemyRoomIndex = rnd.Next(0, levelInfo.EnemyRooms.Length);
                level[i].AddRoomInfo(levelInfo.EnemyRooms[enemyRoomIndex]);
            }

            // Добавление комнаты с боссом
            level[mainPathRoomsCount + 1].AddRoomInfo(levelInfo.BossRoom);

            // Добавление дополнительных комнат с противниками 
            for (int i = mainPathRoomsCount + 2; i <= enemyRoomsCount + 1; i++)
            {
                var enemyRoomIndex = rnd.Next(0, levelInfo.EnemyRooms.Length);
                level[i].AddRoomInfo(levelInfo.EnemyRooms[enemyRoomIndex]);
            }

            // Добавление комнат с лутом
            for (int i = enemyRoomsCount + 2; i < roomsCount; i++)
            {
                var lootRoomIndex = rnd.Next(0, levelInfo.LootRooms.Length);
                level[i].AddRoomInfo(levelInfo.LootRooms[lootRoomIndex]);
            }
            #endregion

            level.ConnectLevelGraph();
            return level;
        }
    }

    public class RoomNode
    {
        // Это Node в Graph

        private readonly List<RoomNode> connectedRooms = new List<RoomNode>();
        public readonly int RoomNumber;

        public RoomInfo RoomInfo;

        public RoomNode(int number)
        {
            RoomNumber = number;
        }

        public IEnumerable<RoomNode> ConnectedRooms
        {
            get
            {
                return connectedRooms;
            }
        }
        public static void Connect(RoomNode node1, RoomNode node2, LevelGraph graph)
        {
            if (!graph.Rooms.Contains(node1) || !graph.Rooms.Contains(node2)) throw new ArgumentException();
            // Соединяет комнаты только если они не соеденины
            if (!node1.connectedRooms.Contains(node2))
            {
                node1.connectedRooms.Add(node2);
                node2.connectedRooms.Add(node1);
            }
        }

        public void AddRoomInfo(RoomInfo roomInfo)
        {
            RoomInfo = roomInfo;
        }
    }

    public class LevelGraph
    {
        // Это Graph

        private int _mainPathRoomsCount;
        private int _enemyRoomsCount;
        private int _lootRoomsCount;
        private int _roomsCount;

        // Уровень:
        //      Уровень состоит из главного пути и дополнительных путей
        //      Главный путь состоит из стартовой комнаты, mainPathRoomsCount комнат с противниками и комнаты с боссом
        //      В комнату с боссом ведёт только 1 проход
        //      Дополнительные пути это отвитвления от главного пути
        //      Дополнительные пути могут появляться начиная со стартовой комнаты
        //      Дополнительные пути могут состоять из комнат с противниками и комнат с лутом
        //      Количество комнат - 2 + enemyRoomsCount + lootRoomsCount

        private RoomNode[] rooms;
        
        // Индексация
        // 0 - startRoom
        // [1; mainPathRoomsCount] - enemyRoom
        // mainPathRoomsCount + 1 - bossRoom
        // [mainPathRoomsCount + 2; enemyRoomsCount + 1] - enemyRoom (не на главном пути)
        // [enemyRoomsCount + 2; roomCount) - lootRoom

        public LevelGraph(int enemyRoomsCount, int lootRoomsCount, int mainPathRoomsCount) 
        { 
            _mainPathRoomsCount = mainPathRoomsCount;
            _enemyRoomsCount = enemyRoomsCount;
            _lootRoomsCount = lootRoomsCount;
            _roomsCount = enemyRoomsCount + lootRoomsCount + 2;
            rooms = Enumerable.Range(0, _roomsCount).Select(z => new RoomNode(z)).ToArray();
        }

        public int Length { get { return rooms.Length; } }

        public RoomNode this[int index] { get { return rooms[index]; } }

        public IEnumerable<RoomNode> Rooms
        {
            get
            {
                return rooms;
            }
        }

        public void ConnectLevelGraph()
        {
            var rnd = new Random();
            var connectCoef = 0.85;
            // Рандомное соединение комнат графа
            for (int i = 0; i < _roomsCount; i++)
            {
                if (i == _mainPathRoomsCount + 1) // Пропускает комнату босса
                {
                    continue;
                }
                for (int j = 0; j < _roomsCount; j++)
                {
                    if ((j == _mainPathRoomsCount + 1) || (i == j)) // Пропускает комнату босса и повторающуюся комнату
                    {
                        continue;
                    }
                    if (rnd.NextDouble() > connectCoef)
                    {
                        Connect(i, j);
                    }
                }
            }

            // Соединение комнат главного пути (от StartRoom до BossRoom через EnemyRooms)
            for (int i = 1; i <= _mainPathRoomsCount + 1; i++)
            {
                Connect(i - 1, i);
            }

            // Проверяем нет ли комнат не соединённых с основным путём
            var conRoomsNum = GetConnectedRoomsNum();
            while (conRoomsNum.Count != _roomsCount)
            {
                foreach (var room in rooms)
                {
                    if (!conRoomsNum.Contains(room.RoomNumber))
                    {
                        var randomNum = rnd.Next(0, _roomsCount);
                        while (randomNum == _mainPathRoomsCount + 1)
                        {
                            randomNum = rnd.Next(0, _roomsCount);
                        }
                        Connect(randomNum, room.RoomNumber);
                        conRoomsNum = GetConnectedRoomsNum();
                    }
                }
            }
        }

        public List<int> GetConnectedRoomsNum()
        {
            var visited = new HashSet<RoomNode>();
            var queue = new Queue<RoomNode>();
            var conRoomsNum = new List<int>();
            queue.Enqueue(rooms[0]);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                if (visited.Contains(node))
                    continue;
                visited.Add(node);
                conRoomsNum.Add(node.RoomNumber);
                foreach (var nextNode in node.ConnectedRooms)
                    queue.Enqueue(nextNode);
            }

            return conRoomsNum;     // Возвращает лист номеров комнат соединённых с основным путём
        }


        public void Connect(int index1, int index2)
        {
            RoomNode.Connect(rooms[index1], rooms[index2], this);
        }
    }
}
