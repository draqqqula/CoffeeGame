using BehaviorKit;
using CoffeeProject.Family;
using CoffeeProject.GameObjects;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.ComponentModel;

namespace CoffeeProject.Weapons
{
    public class TornadoAbility : IPlayerAbility
    {
        private const double RefreshTime = 10;
        private const double Interval = 0.4;
        private const int Count = 5;
        private bool OnCooldown { get; set; } = false;
        public void UseAbility(IControllerProvider state, Hero player)
        {
            if (OnCooldown)
            {
                return;
            }
            OnCooldown = true;
            for (int i = 0; i < Count; i++)
            {
                if (i == 0)
                {
                    PlayerDamageBall.CastBall(state, player.Position, player, i);
                }
                else
                {
                    var index = i;
                    player.GetComponents<TimerHandler>().First()
                    .SetTimer($"createBall{i}",
                    TimeSpan.FromSeconds(Interval * i),
                    () =>
                    { 
                        PlayerDamageBall.CastBall(state, player.Position, player, index); 
                    },
                    true);
                }
            }
            player.GetComponents<TimerHandler>().First().SetTimer("refreshAbility", TimeSpan.FromSeconds(RefreshTime), () => OnCooldown = false, true);
        }
    }
}
