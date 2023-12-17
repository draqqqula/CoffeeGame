using CoffeeProject.GameObjects;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
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

    public class DamageContainer : ComponentBase, IModifiableContainer
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
