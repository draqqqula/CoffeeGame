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
using Microsoft.Xna.Framework.Graphics;

using PVRTexLibNET;
using AsepriteDotNet.Common;
using Color = System.Drawing.Color;
using SharpFont;
using static System.Net.Mime.MediaTypeNames;

namespace CoffeeProject.RoomGeneration
{
    public class LevelGenerator
    {
        private IControllerProvider _state;
        public LevelGenerator(IControllerProvider state)
        {
            _state = state;
        }

        public GraphInfo GenerateLevelGraph(string levelName, int mainPathRoomsCount, int enemyRoomsCount, int lootRoomsCount)
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

            return CreateLevelPicture(level);
        }

        public GraphInfo CreateLevelPicture(LevelGraph levelGraph)
        {
            var posMemory = new Dictionary<int, Rectangle>();
            var BMscale = 300;
            var levelBitmap = new Bitmap(BMscale, BMscale);
            var graphics = Graphics.FromImage(levelBitmap);
            var initX = 100;
            var initY = 200;
            var prevY = 200;
            var rnd = new Random();
            var memory = new Dictionary<int, Dictionary<string, int>>();

            for (int i = 0; i < levelGraph.MainPathRoomsCount; i++)
            {
                memory.Add(i, new Dictionary<string, int>());
            }

            for (int i = 0; i < BMscale; i++)
            {
                for (int j = 0; j < BMscale; j++)
                {
                    levelBitmap.SetPixel(i, j, Color.White);
                }
            }

            // Подстановка Image в Bitmap последовательно
            #region

            var prevNode = levelGraph[0];
            for (int i = 0; i < levelGraph.Length; i++)
            {
                var width = levelGraph[i].RoomInfo.TileMap.Width;
                var height = levelGraph[i].RoomInfo.TileMap.Height;
                //Получение System.Drawing.Image для подачи в Bitmap
                #region
                var colors2d = new Microsoft.Xna.Framework.Color[levelGraph[i].RoomInfo.TileMap.Width, levelGraph[i].RoomInfo.TileMap.Height];
                var colors = new Microsoft.Xna.Framework.Color[levelGraph[i].RoomInfo.TileMap.Width * levelGraph[i].RoomInfo.TileMap.Height];
                levelGraph[i].RoomInfo.TileMap.GetData(colors);
                for (int j = 0; j < levelGraph[i].RoomInfo.TileMap.Width; j++)
                {
                    for (int k = 0; k < levelGraph[i].RoomInfo.TileMap.Height; k++)
                    {
                        colors2d[j, k] = colors[k * levelGraph[i].RoomInfo.TileMap.Width + j];
                    }
                }

                var bitmap = new Bitmap(levelGraph[i].RoomInfo.TileMap.Width, levelGraph[i].RoomInfo.TileMap.Height);
                for (int j = 0; j < levelGraph[i].RoomInfo.TileMap.Width; j++)
                {
                    for (int k = 0; k < levelGraph[i].RoomInfo.TileMap.Height; k++)
                    {
                        bitmap.SetPixel(j, k, Color.FromArgb(colors2d[j, k].R, colors2d[j, k].G, colors2d[j, k].B));
                    }
                }
                var img = (System.Drawing.Image)bitmap;
                #endregion

                if (levelGraph[i].RoomNumber == 0)
                {
                    graphics.DrawImage(img, initX, prevY);
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(initX, initY, width, height));
                }
                if ((levelGraph[i].RoomNumber - 1 == prevNode.RoomNumber) && (levelGraph[i].RoomNumber <= levelGraph.MainPathRoomsCount))
                {
                    var constDelta = 5;
                    var dY = constDelta + levelGraph[i].RoomInfo.TileMap.Height;
                    graphics.DrawImage(img, initX, prevY - dY);
                    prevNode = levelGraph[i];
                    prevY -= dY;
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(initX, prevY, width, height));
                }
                if (levelGraph[i].RoomNumber == levelGraph.MainPathRoomsCount + 1)
                {
                    var bossY = initY - levelGraph.MainPathRoomsCount * 25;
                    graphics.DrawImage(img, initX, bossY);
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(initX, bossY, width, height));
                }
                if (levelGraph[i].RoomNumber > levelGraph.MainPathRoomsCount + 1)
                {
                    string[] sideChose = ["left", "right"];
                    var randomMainPathRoomNum = rnd.Next(levelGraph.MainPathRoomsCount);
                    var randomSide = sideChose[rnd.Next(2)];
                    if (!memory[randomMainPathRoomNum].ContainsKey(randomSide))
                        memory[randomMainPathRoomNum].Add(randomSide, 1);
                    else
                    {
                        memory[randomMainPathRoomNum][randomSide] += 1;
                    }
                    var constDelta = 5;
                    var dX = constDelta + levelGraph[i].RoomInfo.TileMap.Width;
                    if (randomSide == "left")
                        dX = -dX;
                    var thisX = initX + dX * memory[randomMainPathRoomNum][randomSide];
                    graphics.DrawImage(img, thisX, posMemory[randomMainPathRoomNum].Y);
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(thisX, posMemory[randomMainPathRoomNum].Y, width, height));
                }
                levelBitmap.Save($@"F:\TestLevelFinalOutIMG{i}-{levelGraph[i].RoomNumber}.png");
            }

            #endregion

            levelBitmap.Save(@"F:\TestLevelFinalOutIMG.png");
            // Перевод конечной картинки в двумерный массив Xna.Color
            var finalColorArray = new Microsoft.Xna.Framework.Color[levelBitmap.Width, levelBitmap.Height];
            for (int i = 0; i < levelBitmap.Width; i++)
            {
                for (int j = 0; j < levelBitmap.Height; j++)
                {
                    var color = levelBitmap.GetPixel(i, j);
                    finalColorArray[i, j] = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
                }
            }
            return new GraphInfo(finalColorArray, posMemory, levelGraph);
        }
    }
    public record GraphInfo(Microsoft.Xna.Framework.Color[,] Colors, Dictionary<int, Rectangle> Positions, LevelGraph Graph);


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
        public int MainPathRoomsCount { get {  return _mainPathRoomsCount; } }

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
