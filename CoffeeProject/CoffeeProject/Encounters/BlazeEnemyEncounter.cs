using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeProject.SurfaceMapping;
using CoffeeProject.RoomGeneration;
using CoffeeProject.CustomNodes;
using BehaviorKit;

namespace CoffeeProject.Encounters
{
    public class BlazeEnemyEncounter : Encounter
    {
        public BlazeEnemyEncounter(int level) : base(level)
        {
        }

        public override void Invoke(IControllerProvider state, Vector2 position, Room room)
        {
            var enemy = state.Using<IFactoryController>()
            .CreateObject<Ben>()
            .SetPos(position)
            .SetBounds(new Rectangle(-20, -40, 40, 40))
            .SetPlacement(Placement<MainLayer>.On())
            .SetLevel(Level)
            .AddComponent(new Dummy(
                12 + Level * 4, [], Team.enemy, [], [], 1
                ))
            .AddHealthLabel(state)
            .RandomizeElement()
            .AddShadow(state)
            .AddToState(state);
            enemy.InvokeEach<Physics>(it => it.SurfaceMap = state.Using<SurfaceMapProvider>().GetMap("level"));
            room.Enemies.Add(enemy);
            enemy.OnDisposeEvent += (obj) => room.Enemies.Remove(enemy);
        }
    }
}
