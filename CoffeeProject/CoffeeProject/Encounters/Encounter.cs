using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
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
        public abstract void Create(IControllerProvider state, Point position);
    }

    public class EnemyEncounter : Encounter
    {
        public override void Create(IControllerProvider state, Point position)
        {
        }
    }
}
