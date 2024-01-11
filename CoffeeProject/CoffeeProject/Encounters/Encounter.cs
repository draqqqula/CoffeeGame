using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.RoomGeneration;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Encounters
{
    public abstract class Encounter
    {
        public abstract void Invoke(IControllerProvider state, Vector2 position, Room room);
    }
}
