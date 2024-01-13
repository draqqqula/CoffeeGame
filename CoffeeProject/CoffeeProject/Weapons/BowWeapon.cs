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
    public class BowWeapon : IPlayerWeapon
    {
        public double AttackInterval { get; set; } = 0.9;
        public int AttackSize { get; set; } = 130;
        public float DirectionOffset { get; set; } = 70;
        public DamageInstance Damage { get; set; }
        public void UsePrimary(IControllerProvider state, Hero player)
        {
            var timer = player.GetComponents<TimerHandler>().First();
            var physics = player.GetComponents<Physics>().Last();
            if (timer.OnLoop("shoot", TimeSpan.FromSeconds(AttackInterval), delegate { }))
            {
                var offset = Enum.Parse<Direction>(player.Animator.Running.Name.Replace("Default", "Forward")).ToPoint().ToVector2() * DirectionOffset;
                PlayerProjectile.ShootProjectile(state, player.Position, Enum.Parse<Direction>(player.Animator.Running.Name.Replace("Default", "Forward")), player.GetComponents<Dummy>().First(), "Arrow");

                var damage = new Dictionary<DamageType, int>
                    {
                        { DamageType.Physical, 2 }
                    };
                player.Animator.SetAnimation("Atk_Bow_" + player.Animator.Running.Name, 0);
                player.Animator.Resume();
                player.DirectionAnimationForced = false;
                state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "hero_attack"), "hero_attack").Play();
            }
        }
    }
}
