using CoffeeProject.GameObjects;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Behaviors
{
    public interface IModifiableContainer
    {
        public void SetDefaults();
    }

    public class DamageContainer : Behavior<Hero>, IModifiableContainer
    {
        public int FireDamageDefault = 4;
        public int WaterDamageDefault = 12;
        public int FireDamage = 4;
        public int WaterDamage = 12;

        public void SetDefaults()
        {
            FireDamage = FireDamageDefault;
            WaterDamage = WaterDamageDefault;
        }
    }
}
