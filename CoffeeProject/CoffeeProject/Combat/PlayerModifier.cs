using CoffeeProject.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Combat
{
    public abstract class PlayerModifier
    {
        public readonly bool HasUpdate = true;
        public abstract void ApplyOnUpdate(Hero hero);
        public abstract void ApplyOnce(Hero hero);
        public abstract void Drop(Hero hero);

        public PlayerModifier(bool hasUpdate)
        {
            HasUpdate = hasUpdate;
        }
    }
}
