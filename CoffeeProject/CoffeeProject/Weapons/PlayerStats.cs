using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Weapons
{
    public record class PlayerStats
    {
        public int Currency { get; set; } = 0;
        public int AttackPower { get; set; } = 0;
        public int HealthPower { get; set; } = 0;
        public int Speed { get; set; } = 0;
    }
}
