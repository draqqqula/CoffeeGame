using BehaviorKit;
using CoffeeProject.CustomNodes;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using System;
using MagicDustLibrary.Factorys;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Elements
{
    public class HolySpawner : Behavior<IBodyComponent>
    {
        private TimerHandler Timer { get; set; }
        public HolySpawner()
        {
            AddGreetingFor<TimerHandler>(it => Timer = it);
        }
        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IBodyComponent parent)
        {
            Timer.OnLoop("HealOverTime", TimeSpan.FromSeconds(1.5), () =>
            {
                if (parent is GameObject gobj)
                {
                    gobj.GetComponents<Dummy>().First().RecieveHealing(1);
                }
            });
        }
    }
}
