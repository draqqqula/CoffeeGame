using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.Organization;
using System.Text.Json;

namespace CoffeeProject.Weapons
{
    public class CharacterBuilder
    {
        public Hero Build(IControllerProvider state, BuildConfiguration args, GameClient client, Vector2 position, LevelArgs arguments)
        {
            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");

            var healthIndicator = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(100, 20))
                .UseCustomFont(state, "HeartFont")
                .SetScale(0.4f)
                .SetText("c")
                .AddToState(state);
            var obj = state.Using<IFactoryController>().CreateObject<Hero>()
                .SetPos(position)
                .SetBounds(new Rectangle(-20, -40, 40, 40))
                .SetPlacement(Placement<MainLayer>.On())
                .AddComponent(new TimerHandler())
                .AddComponent(new Playable(healthIndicator))
                .AddShadow(state)
                .UseWeapon(BuildWeapon(args.Weapon))
                .UseElement(BuildElement(args.Element))
                .UseAbility(BuildAbility(args.Ability))
                .AddToState(state);

            obj.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);
            var dummy = obj.GetComponents<Dummy>().First();

            obj.Client = client;
            obj.LevelArgs = arguments;
            if (arguments.Data.Length > 2)
            {
                var stats = JsonSerializer.Deserialize<PlayerStats>(arguments.Data[2]);
                obj.Stats = stats;
            }

            state.Using<IClientController>().AttachCamera(client, obj);

            return obj;
        }

        private IPlayerWeapon BuildWeapon(ChosenWeapon? chosen)
        {
            switch (chosen)
            {
                case ChosenWeapon.Knife:
                    return new KnifeWeapon();
                case ChosenWeapon.Axe:
                    return new ShowelWeapon();
                case ChosenWeapon.Sword:
                    return new PickaxeWeapon();
                case ChosenWeapon.Bow:
                    return new BowWeapon();
                default:
                    return new KnifeWeapon();
            }
        }

        private IPlayerAbility BuildAbility(ChosenAbility? chosen)
        {
            switch (chosen)
            {
                case ChosenAbility.Dash:
                    return new DashAbility();
                case ChosenAbility.Block:
                    return new TimeStopAbility();
                case ChosenAbility.Cleanse:
                    return new TornadoAbility();
                default:
                    return new DashAbility();
            }
        }

        private DamageType BuildElement(ChosenElement? chosen)
        {
            switch (chosen)
            {
                case ChosenElement.Fire:
                    return DamageType.Fire;
                case ChosenElement.Ice:
                    return DamageType.Ice;
                case ChosenElement.Light:
                    return DamageType.Light;
                case ChosenElement.Dark:
                    return DamageType.Dark;
                default:
                    return DamageType.Physical;
            }
        }
    }
}
