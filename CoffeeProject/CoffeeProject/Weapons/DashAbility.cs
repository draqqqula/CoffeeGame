using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Weapons
{
    public class DashAbility : IPlayerAbility
    {
        public class BlackFilter : NodeComponent, IDisplayFilter
        {
            public float Intensity { get; set; } = 1;
            public DrawingParameters ApplyFilter(DrawingParameters info)
            {
                return info with { Color = new Color(1 - Intensity, 1 - Intensity, 1 - Intensity) };
            }
        }

        public float Speed = 4f;
        public float Acceleration = -1f;
        public double LifeTime = 0.4;
        public double RefreshTime = 2.4;
        public double BlackFilterDuration = 0.1;

        private bool OnCooldown { get; set; } = false;
        public void UseAbility(IControllerProvider state, Hero player)
        {
            if (OnCooldown)
            {
                return;
            }
            var resultingVector = Vector2.Zero;
            foreach (var direction in player.Directions)
            {
                resultingVector += direction.ToPoint().ToVector2();
            }
            resultingVector.Normalize();
            if (resultingVector == Vector2.Zero)
            {
                return;
            }
            player.InvokeEach<Physics>(it => 
            it.AddVector("dash", 
            new MovementVector(resultingVector * Speed, Acceleration, TimeSpan.FromSeconds(LifeTime), 
            false)));
            OnCooldown = true;
            player.InvokeEach<Dummy>(it => it.IsInvincible = true);
            state.Using<IFactoryController>()
                .CreateObject<DamageBallCollapse>()
                .SetPlacement(new Placement<MainLayer>())
                .SetPos(player.Position)
                .AddComponent(new BlackFilter())
                .AddToState(state);

            player.Invisible = true;
            player.WeaponInputBlocked = true;
            player.InvokeEach<TimerHandler>(it => it.SetTimer("refreshDash", RefreshTime, () =>
            {
                OnCooldown = false;
                player.AddComponent(new BlackFilter());
                player.InvokeEach<TimerHandler>(it => it.SetTimer("removeBlackCooldownFilter", BlackFilterDuration, () => player.Without<BlackFilter>(), true));
            }
            , true));
            player.InvokeEach<TimerHandler>(it => it.SetTimer("deleteInvincibility", LifeTime, () => 
            {
                player.WeaponInputBlocked = false;
                player.InvokeEach<Dummy>(it => it.IsInvincible = false);
                player.Invisible = false;
                state.Using<IFactoryController>()
                .CreateObject<DamageBallCollapse>()
                .SetPlacement(new Placement<MainLayer>())
                .SetPos(player.Position)
                .AddComponent(new BlackFilter())
                .AddToState(state);
                player
                .AddComponent(new BlackFilter());
            }, true));
            player.InvokeEach<TimerHandler>(it => it.SetTimer("removeBlackFilter", LifeTime + BlackFilterDuration, () => player.Without<BlackFilter>(), true));
        }
    }
}
