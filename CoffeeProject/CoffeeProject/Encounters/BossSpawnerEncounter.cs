using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.ComponentModel;
using CoffeeProject.SurfaceMapping;
using CoffeeProject.RoomGeneration;

namespace CoffeeProject.Encounters
{
    public class BossSpawnerEncounter : Encounter
    {
        public BossSpawnerEncounter(int level) : base(level)
        {
        }

        public override void Invoke(IControllerProvider state, Vector2 position, Room room)
        {
            var spawner = state.Using<IFactoryController>()
                .CreateObject<BossSpawner>()
                .SetPos(position)
                .SetBounds(new Rectangle(-20, -20, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddShadow(state)
                .AddToState(state);
            spawner.Level = Level;
            spawner.InvokeEach<Physics>(it => it.SurfaceMap = state.Using<SurfaceMapProvider>().GetMap("level"));
            room.Trigger.PlayerDetected += (player) => spawner.Target = player;
        }
    }
}
