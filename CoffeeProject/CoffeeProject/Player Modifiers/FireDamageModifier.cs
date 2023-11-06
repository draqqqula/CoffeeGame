using CoffeeProject.Behaviors;
using CoffeeProject.Combat;
using CoffeeProject.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Player_Modifiers
{
    internal class FireDamageModifier : PlayerModifier
    {
        public override void ApplyOnce(Hero hero)
        {
            var container = hero.GetBehavior<DamageContainer>("container");
            container.FireDamageDefault += 4;
        }

        public override void ApplyOnUpdate(Hero hero)
        {
        }

        public override void Drop(Hero hero)
        {
            var container = hero.GetBehavior<DamageContainer>("container");
            container.FireDamageDefault -= 4;
        }

        public FireDamageModifier() : base(false)
        {
        }
    }
}
