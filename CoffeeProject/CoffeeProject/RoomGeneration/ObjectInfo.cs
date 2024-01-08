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
using GraphShape.Utils;

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
            var pictureScale = 1.0;
            if (enemyRoomsCount + lootRoomsCount - mainPathRoomsCount > 5) pictureScale = 1.5;

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
                //var lootRoomIndex = rnd.Next(0, levelInfo.LootRooms.Length);
                //level[i].AddRoomInfo(levelInfo.LootRooms[lootRoomIndex]);
                level[i].AddRoomInfo(levelInfo.LootRooms[1]);
            }
            #endregion

            return CreateLevelPicture(level, pictureScale);
        }

        public GraphInfo CreateLevelPicture(LevelGraph levelGraph, double pictureScale)
        {
            var posMemory = new Dictionary<int, Rectangle>();
            var BMscale = (int)(400 * pictureScale);

            var levelBitmap = new Bitmap(BMscale, BMscale);
            var graphics = Graphics.FromImage(levelBitmap);
            var initX = BMscale / 2;
            var initY = BMscale - 50;
            var prevY = BMscale - 50;
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
            var constDelta = 10;
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
                    levelGraph.Connect(i, i - 1);
                    var dY = constDelta + levelGraph[i].RoomInfo.TileMap.Height;
                    graphics.DrawImage(img, initX, prevY - dY);
                    prevNode = levelGraph[i];
                    prevY -= dY;
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(initX, prevY, width, height));
                }
                if (levelGraph[i].RoomNumber == levelGraph.MainPathRoomsCount + 1)
                {
                    levelGraph.Connect(i, i - 1);
                    var bossY = posMemory[i - 1].Y - constDelta - levelGraph[i].RoomInfo.TileMap.Height;
                    graphics.DrawImage(img, initX, bossY);
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(initX, bossY, width, height));
                }
                if (levelGraph[i].RoomNumber > levelGraph.MainPathRoomsCount + 1)
                {
                    string[] sideChose = ["left", "right"];
                    var randomMainPathRoomNum = rnd.Next(levelGraph.MainPathRoomsCount);

                    var randomSide = sideChose[rnd.Next(2)];
                    if (!memory[randomMainPathRoomNum].ContainsKey(randomSide))
                        memory[randomMainPathRoomNum].Add(randomSide, randomMainPathRoomNum);

                    var dX = levelGraph[memory[randomMainPathRoomNum][randomSide]].RoomInfo.TileMap.Width + levelGraph[i].RoomInfo.TileMap.Width;
                    if (randomSide == "left")
                        dX = -dX;
                    var thisX = dX + posMemory[memory[randomMainPathRoomNum][randomSide]].X;
                    graphics.DrawImage(img, thisX, posMemory[randomMainPathRoomNum].Y);
                    posMemory.Add(levelGraph[i].RoomNumber, new Rectangle(thisX, posMemory[randomMainPathRoomNum].Y, width, height));
                    levelGraph.Connect(i, memory[randomMainPathRoomNum][randomSide]);
                    memory[randomMainPathRoomNum][randomSide] = levelGraph[i].RoomNumber;
                }
            }
            if (levelGraph.Length - levelGraph.MainPathRoomsCount > 4)
            {
                for (int j = levelGraph.MainPathRoomsCount + 2; j < levelGraph.Length - 1; j++)
                {
                    for (int k = levelGraph.MainPathRoomsCount + 2; k < levelGraph.Length - 1; k++)
                    {
                        if (j == k) continue;
                        if (Math.Abs(posMemory[j].X - posMemory[k].X) < 16 && Math.Abs(posMemory[j].Y - posMemory[k].Y) < 35)
                        {
                            var checkConnection = rnd.NextDouble();
                            if (checkConnection > 0.50)
                            {
                                levelGraph.Connect(k, j);
                            }
                        }
                    }
                }
            }

            #endregion

            levelBitmap.Save(@"TestLevelFinalOutIMG.png");
            levelBitmap = CorridorCreator.CreateCorridors(levelBitmap, graphics, posMemory, levelGraph);
            levelBitmap.Save(@"TestLevelFinalOutIMG2.png");
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

    public class CorridorCreator
    {
        public static Bitmap CreateCorridors(Bitmap levelBitmap, Graphics graphics, Dictionary<int, Rectangle> posMemory, LevelGraph levelGraph)
        {
            var borderColor = Color.FromArgb(255, 165, 0);
            var wallColor = Color.FromArgb(255, 0, 0);
            var floorColor = Color.FromArgb(0, 0, 0);
            var corridorsTest = new List<(int, int)>();
            var corridors = new Dictionary<RoomNode, List<RoomNode>>();
            for (int i = 0; i < levelGraph.Rooms.Count(); i++)
            {
                for (int j = 0; j < levelGraph[i].ConnectedRooms.Count(); j++)
                {
                    var firstRoomPos = posMemory[i];
                    var secondRoomPos = posMemory[levelGraph[i].ConnectedRooms.ElementAt(j).RoomNumber];
                    var bestGates = ChoseBestGates(levelGraph, i, j, firstRoomPos, secondRoomPos);

                    var firstGate = new Point(levelGraph[i].RoomInfo.Gates[bestGates.Item1].X + firstRoomPos.X, levelGraph[i].RoomInfo.Gates[bestGates.Item1].Y + firstRoomPos.Y);
                    var secondGate = new Point(levelGraph[i].ConnectedRooms.ElementAt(j).RoomInfo.Gates[bestGates.Item2].X + secondRoomPos.X, levelGraph[i].ConnectedRooms.ElementAt(j).RoomInfo.Gates[bestGates.Item2].Y + secondRoomPos.Y);
                    var dirX = 1;
                    var dirY = 1;
                    if (secondGate.X < firstGate.X)
                    {
                        dirX = -dirX;
                    }
                    if (secondGate.Y < firstGate.Y)
                    {
                        dirY = -dirY;
                    }

                    var deltaX = Math.Abs(firstGate.X - secondGate.X);
                    var deltaY = Math.Abs(firstGate.Y - secondGate.Y);

                    if (i > levelGraph.MainPathRoomsCount + 1 && dirY > 0) continue;

                    if (!corridorsTest.Contains((levelGraph[i].ConnectedRooms.ElementAt(j).RoomNumber, i)))
                    {
                        corridorsTest.Add((i, levelGraph[i].ConnectedRooms.ElementAt(j).RoomNumber));
                    }
                    else continue;

                    if (CheckHorizontal(levelBitmap, firstGate))
                    {
                        levelBitmap.SetPixel(firstGate.X, firstGate.Y + 1 * -dirY, floorColor);
                        levelBitmap.SetPixel(firstGate.X, firstGate.Y + 2 * -dirY, floorColor);
                        for (int dY = 0; dY <= deltaY / 2; dY++)
                        {
                            levelBitmap.SetPixel(firstGate.X, firstGate.Y + dY * dirY, floorColor);
                            levelBitmap.SetPixel(firstGate.X - 1, firstGate.Y + dY * dirY, borderColor);
                            levelBitmap.SetPixel(firstGate.X + 1, firstGate.Y + dY * dirY, borderColor);
                        }
                        for (int dX = 0; dX <= deltaX; dX++)
                        {
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY, floorColor);

                            if (dX != 0)
                                levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY + 1, borderColor);
                            else 
                            {
                                levelBitmap.SetPixel(firstGate.X - dirX, firstGate.Y + deltaY / 2 * dirY, borderColor);
                                levelBitmap.SetPixel(firstGate.X - dirX, firstGate.Y + deltaY / 2 * dirY - 1, borderColor);
                                levelBitmap.SetPixel(firstGate.X - dirX, firstGate.Y + deltaY / 2 * dirY - 2, borderColor);
                                levelBitmap.SetPixel(firstGate.X - dirX, firstGate.Y + deltaY / 2 * dirY - 3, borderColor);
                            }
                            if (dX != deltaX)
                            {
                                levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY - 1, wallColor);
                                levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY - 2, wallColor);
                                levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY - 3, borderColor);
                            }
                            else
                            {
                                levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY - 1, floorColor);
                                levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY / 2 * dirY - 2, floorColor);

                                levelBitmap.SetPixel(firstGate.X + dX * dirX + dirX, firstGate.Y + deltaY / 2 * dirY + 1, borderColor);
                                levelBitmap.SetPixel(firstGate.X + dX * dirX + dirX, firstGate.Y + deltaY / 2 * dirY, borderColor);
                                levelBitmap.SetPixel(firstGate.X + dX * dirX + dirX, firstGate.Y + deltaY / 2 * dirY - 1, borderColor);
                                levelBitmap.SetPixel(firstGate.X + dX * dirX + dirX, firstGate.Y + deltaY / 2 * dirY - 2, borderColor);
                            }
                        }
                        for (int dY = deltaY / 2 + 3; dY <= deltaY; dY++)
                        {
                            levelBitmap.SetPixel(firstGate.X + deltaX * dirX, firstGate.Y + dY * dirY, floorColor);
                            levelBitmap.SetPixel(firstGate.X + deltaX * dirX - 1, firstGate.Y + dY * dirY, borderColor);
                            levelBitmap.SetPixel(firstGate.X + deltaX * dirX + 1, firstGate.Y + dY * dirY, borderColor);
                        }
                    }
                    else
                    {
                        for (int dX = 0; dX <= deltaX / 2; dX++)
                        {
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y, floorColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + 1, borderColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y - 1, wallColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y - 2, wallColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y - 3, borderColor);
                        }
                        for (int dY = 0; dY <= deltaY; dY++)
                        {
                            levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY, floorColor);
                            if (deltaY == 0) break;
                            if (deltaY == 1)
                            {
                                if (dirY > 0)
                                {
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY - 3, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY, floorColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY + 1, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY + dirY + 1, borderColor);
                                }
                                else
                                {
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY - dirY, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY, floorColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY - 1, wallColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY - 2, wallColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY - 3, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY + dirY - 3, borderColor);
                                }
                                break;
                            }
                            if (dY == 0)
                            {
                                if (dirY < 0)
                                {
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY - dirY, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY - dirY, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY, borderColor);
                                }
                                else
                                {
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY - 3, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY - 2, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY - 1, borderColor);
                                    levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY, borderColor);
                                }
                            }
                            if (dY > 2 && dirY < 0)
                            {
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY, borderColor);
                            }
                            if (dY > 1 && dirY > 0)
                            {
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY, borderColor);
                            }
                            if (dY < deltaY)
                            {
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX + dirX, firstGate.Y + dY * dirY, borderColor);
                            }
                            if (dY == deltaY && deltaY > 1 && dirY < 0)
                            {
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY - 1, wallColor);
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY - 2, wallColor);
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY - 3, borderColor);

                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY - 1, borderColor);
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY - 2, borderColor);
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY - 3, borderColor);
                            }
                            if (dY == deltaY && deltaY > 1 && dirY > 0)
                            {
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX - dirX, firstGate.Y + dY * dirY + dirY, borderColor);
                                levelBitmap.SetPixel(firstGate.X + deltaX / 2 * dirX, firstGate.Y + dY * dirY + dirY, borderColor);
                            }
                        }
                        for (int dX = deltaX / 2 + 1; dX <= deltaX; dX++)
                        {
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY * dirY, floorColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY * dirY + 1, borderColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY * dirY - 1, wallColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY * dirY - 2, wallColor);
                            levelBitmap.SetPixel(firstGate.X + dX * dirX, firstGate.Y + deltaY * dirY - 3, borderColor);
                        }
                    }

                }
            }
            return levelBitmap;
        }

        private static bool CheckHorizontal(Bitmap levelBitmap, Point gate)
        {
            var leftColor = levelBitmap.GetPixel(gate.X - 1, gate.Y);
            var rightColor = levelBitmap.GetPixel(gate.X + 1, gate.Y);
            return leftColor == rightColor;
        }

        private static (int, int) ChoseBestGates(LevelGraph levelGraph, int i, int j, Rectangle firstRoomPos, Rectangle secondRoomPos)
        {
            var gatesDifferents = new List<(int, int, double)>();
            for (int k = 0; k < levelGraph[i].RoomInfo.Gates.Length; k++)
            {
                for (int l = 0; l < levelGraph[i].ConnectedRooms.ElementAt(j).RoomInfo.Gates.Length; l++)
                {
                    var firstGatePos = new Point(firstRoomPos.X + levelGraph[i].RoomInfo.Gates[k].X, firstRoomPos.Y + levelGraph[i].RoomInfo.Gates[k].Y);
                    var secondGatePos = new Point(secondRoomPos.X + levelGraph[i].ConnectedRooms.ElementAt(j).RoomInfo.Gates[l].X, secondRoomPos.Y + levelGraph[i].ConnectedRooms.ElementAt(j).RoomInfo.Gates[l].Y);
                    var difX = Math.Abs((firstGatePos - secondGatePos).X);
                    var difY = Math.Abs((firstGatePos - secondGatePos).Y);
                    var range = Math.Sqrt(difY * difY + difX * difX);
                    gatesDifferents.Add((k, l, range));
                }
            }
            var bestGates = (-1, -1);
            var gateMinDif = double.MaxValue;
            for (int k = 0; k < gatesDifferents.Count; k++)
            {
                gateMinDif = Math.Min(gateMinDif, gatesDifferents[k].Item3);
            }
            for (int k = 0; k < gatesDifferents.Count; k++)
            {
                if (gatesDifferents[k].Item3 == gateMinDif)
                {
                    bestGates = (gatesDifferents[k].Item1, gatesDifferents[k].Item2);
                    break;
                }
            }

            return bestGates;
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

        public void Connect(int index1, int index2)
        {
            RoomNode.Connect(rooms[index1], rooms[index2], this);
        }
    }
}
