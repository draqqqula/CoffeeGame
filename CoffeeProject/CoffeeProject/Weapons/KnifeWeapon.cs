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
using CoffeeProject.CustomNodes;
namespace CoffeeProject.Weapons
{
    public class KnifeWeapon : IPlayerWeapon
    {
        public double AttackInterval { get; set; } = 0.4;
        public int AttackSize { get; set; } = 130;
        public float DirectionOffset { get; set; } = 70;
        public DamageInstance Damage { get; set; }
        public void UsePrimary(IControllerProvider state, Hero player)
        {
            var timer = player.GetComponents<TimerHandler>().First();
            var physics = player.GetComponents<Physics>().Last();
            var random = new Random();
            if (timer.OnLoop("slash", TimeSpan.FromSeconds(AttackInterval), delegate { }))
            {
                var offset = Enum.Parse<Direction>(player.Animator.Running.Name.Replace("Default", "Forward")).ToPoint().ToVector2() * DirectionOffset;
                var slash = state.Using<IFactoryController>().CreateObject<Slash>()
                .SetPos(player.Position + offset)
                .SetBounds(new Rectangle(-AttackSize / 2, -AttackSize / 2, AttackSize, AttackSize))
                .SetPlacement(new Placement<MainLayer>())
                .AddComponent(new ElementFilter(player.Element))
                .AddToState(state);
                PlayerProjectile.ShootProjectile(
                    state, player.Position, 
                    Enum.Parse<Direction>(player.Animator.Running.Name.Replace("Default", "Forward")), 
                    player.GetComponents<Dummy>().First(), "Knife", 1 + player.Stats.AttackPower, player.Element, 1);
                var damage = new Dictionary<DamageType, int>
                    {
                        { DamageType.Physical, 1 },
                        { player.Element, 1 }
                    };

                slash.Damage = new DamageInstance(damage, Team.player, [], $"Damage{random.Next()}", player.GetComponents<Dummy>().First(), [], [], TimeSpan.FromSeconds(0.5));
                slash.Offset = offset;
                slash.Animator.SetAnimation(player.Animator.Running.Name, 0);
                slash.Owner = player;
                player.Animator.SetAnimation("Atk_Knife_" + player.Animator.Running.Name, 0);
                player.Animator.Resume();
                player.DirectionAnimationForced = false;
                timer.SetTimer("deleteSlash", TimeSpan.FromSeconds(0.2), () =>
                {
                    slash.Dispose();
                }, true);
                state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "hero_attack"), "hero_attack").Play();
            }
        }
    }
}
