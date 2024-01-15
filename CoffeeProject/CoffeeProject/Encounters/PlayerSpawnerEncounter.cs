using CoffeeProject.Levels;
using CoffeeProject.RoomGeneration;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Encounters
{
    class PlayerSpawnerEncounter(IDungeonLevel level) : Encounter
    {
        private IDungeonLevel Location { get; init; } = level;
        public override void Invoke(IControllerProvider state, Vector2 position, Room room)
        {
            Location.PlayerPosition = position;
        }
    }
}
