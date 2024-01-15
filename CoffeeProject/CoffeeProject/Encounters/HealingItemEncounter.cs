using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.RoomGeneration;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Encounters
{
    public class HealingItemEncounter : Encounter
    {
        public HealingItemEncounter(int level) : base(level)
        {
        }

        public override void Invoke(IControllerProvider state, Vector2 position, Room room)
        {
            state.Using<IFactoryController>()
                .CreateObject<HealingItem>()
                .SetPlacement(new Placement<MainLayer>())
                .SetBounds(new Rectangle(-20, -20, 40, 40))
                .SetPos(position)
                .AddShadow(state)
                .AddToState(state).Amount = 3 + Level * 2;
        }
    }
}
