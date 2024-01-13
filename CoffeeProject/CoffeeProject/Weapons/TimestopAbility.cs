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
using CoffeeProject.Layers;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.CommonObjectTypes.Image;
using CoffeeProject.Behaviors;
using Microsoft.Xna.Framework;

namespace CoffeeProject.Weapons
{
    public class TimeStopAbility : IPlayerAbility
    {
        private const double RefreshTime = 8;
        private const double Duration = 2.5;
        private const double Scale = 0.2;
        private bool OnCooldown { get; set; } = false;
        public void UseAbility(IControllerProvider state, Hero player)
        {
            if (OnCooldown)
            {
                return;
            }
            OnCooldown = true;
            state.Using<ITimeController>().SetStateTimeScale(Scale);
            
            CreateCircleFX(state, player);

            state.Using<ITimeController>().SetStateTimeScale(Scale);
            var immune = state.Using<IFamilyController>().GetFamily<TimeStopImmune>();
            foreach (var obj in immune)
            {
                state.Using<ITimeController>().SetObjectTimeScale(obj, 1/Scale);
            }

            var vignette = state.Using<IFactoryController>()
                .CreateObject<Image>()
                .SetPlacement(new Placement<CenterLayer>())
                .SetTexture("white_vignette")
                .AddToState(state);
            vignette.SetScale(player.Client.Window.Size.ToVector2() / vignette.TextureBounds.Size.ToVector2());
            vignette.AddComponent(new TimeFunction((t) =>
            {
                vignette.SetScale(player.Client.Window.Size.ToVector2() / vignette.TextureBounds.Size.ToVector2());
                vignette.SetOpacity(1 - Convert.ToSingle((t / Scale) / Duration));
            }));
            player.GetComponents<TimerHandler>().First().SetTimer("timeResume", TimeSpan.FromSeconds(Duration), () =>
            { 
                state.Using<ITimeController>().SetStateTimeScale(1);
                var immune = state.Using<IFamilyController>().GetFamily<TimeStopImmune>();
                foreach (var obj in immune)
                {
                    state.Using<ITimeController>().SetObjectTimeScale(obj, 1);
                }
                CreateCircleFX(state, player);
                vignette.Dispose();
            }, true);
            player.GetComponents<TimerHandler>().First().SetTimer("refreshAbility", TimeSpan.FromSeconds(RefreshTime), () => 
            {
                CreateCircleFX(state, player);
                OnCooldown = false;
            }, true);
        }

        private void CreateCircleFX(IControllerProvider state, Hero player)
        {
            var circle = state.Using<IFactoryController>()
            .CreateObject<Image>()
            .SetPlacement(new Placement<FXLayer>())
            .SetTexture("white_circle")
            .SetPos(player.Position)
            .SetScale(Vector2.Zero)
            .AddComponent(new OffsetFilter(new Vector2(0, -70)))
            .AddToState(state);

            circle.AddComponent(new TimeFunction((t) =>
            {
                circle.Position = player.Position;
                var scale = Convert.ToSingle(t / 0.1);
                circle.SetOpacity(1 - scale);
                circle.Scale = new Vector2(scale, scale);
                if (t > 0.1)
                {
                    circle.Dispose();
                }
            }));
        }
    }
}
