using CoffeeProject.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.RoomGeneration
{
    public class Room(RoomTrigger trigger)
    {
        public List<IEnemy> Enemies { get; init; } = [];
        public RoomTrigger Trigger { get; init; } = trigger;
    }
}
