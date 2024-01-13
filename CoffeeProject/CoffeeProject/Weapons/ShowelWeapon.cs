using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Factorys;
using CoffeeProject.BoxDisplay;
namespace CoffeeProject.Weapons
{
    public class ShowelWeapon : IPlayerWeapon
    {
        public double AttackInterval { get; set; } = 1.2;
        public int AttackSize { get; set; } = 240;
        public float DirectionOffset { get; set; } = 100;
        public DamageInstance Damage { get; set; }
        public void UsePrimary(IControllerProvider state, Hero player)
        {
            var timer = player.GetComponents<TimerHandler>().First();
            var physics = player.GetComponents<Physics>().Last();
            if (timer.OnLoop("slash", TimeSpan.FromSeconds(AttackInterval), delegate { }))
            {
                var offset = Enum.Parse<Direction>(player.Animator.Running.Name.Replace("Default", "Forward")).ToPoint().ToVector2() * DirectionOffset;
                var slash = state.Using<IFactoryController>().CreateObject<Slash>()
                .SetPos(player.Position + offset)
                .SetBounds(new Rectangle(-AttackSize / 2, -AttackSize / 2, AttackSize, AttackSize))
                .UseBoxDisplay(state, Color.Red * 0.1f, Color.Purple * 0.1f, 3)
                .SetPlacement(new Placement<MainLayer>())
                .AddToState(state);

                var damage = new Dictionary<DamageType, int>
                    {
                        { DamageType.Physical, 5 }
                    };

                slash.Damage = new DamageInstance(damage, Team.player, [], "Slash", player.GetComponents<Dummy>().First(), [], [], TimeSpan.FromSeconds(0.5));
                slash.Offset = offset;
                slash.Animator.SetAnimation(player.Animator.Running.Name, 0);
                slash.Owner = player;
                timer.SetTimer("deleteSlash", TimeSpan.FromSeconds(0.2), () =>
                {
                    slash.Dispose();
                }, true);
                state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "hero_attack"), "hero_attack").Play();
            }
        }
    }
}
