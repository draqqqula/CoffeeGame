using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.RoomGeneration;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.Extensions;
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
    public class ShopItemEncounter : Encounter
    {
        public ShopItemEncounter(int level) : base(level)
        {
        }

        public override void Invoke(IControllerProvider state, Vector2 position, Room room)
        {
            var random = new RandomEx().NextDouble();
            var cost = Level * 10 + 40;
            var amount = Level;
            PlayerStat stat;
            string statName = "";
            if (random < 1/3)
            {
                stat = PlayerStat.Health; statName = "к здоровью";
            }
            else if (random < 2/3)
            {
                stat = PlayerStat.Attack; statName = "к атаке";
            }
            else
            {
                stat = PlayerStat.Speed; statName = "к скорости";
            }
            var label = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<FXLayer>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.Center)
                .SetScale(0.4f)
                .SetText($"+ {amount} {statName}, {cost} оп.")
                .SetPos(position + Vector2.UnitY * 45f)
                .SetColor(Color.White)
                .AddToState(state);
            var item = state.Using<IFactoryController>()
                .CreateObject<ShopItem>()
                .SetPlacement(new Placement<MainLayer>())
                .SetBounds(new Rectangle(-20, -20, 40, 40))
                .SetPos(position)
                .AddShadow(state)
                .AddToState(state);
            item.Stat = stat;
            item.Label = label;
            item.Amount = amount;
            item.Cost = cost;
        }
    }
}
