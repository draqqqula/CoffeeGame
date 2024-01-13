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

namespace CoffeeProject.Weapons
{
    public class CharacterBuilder
    {
        public Hero Build(IControllerProvider state, BuildConfiguration args, GameClient client, Vector2 position)
        {
            var surfaces = state.Using<SurfaceMapProvider>().GetMap("level");

            var healthIndicator = state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<GUI>())
                .SetPos(new Vector2(100, 50))
                .UseCustomFont(state, "TestFont")
                .SetScale(4f)
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
                .UseAbility(BuildAbility(args.Ability))
                .AddToState(state);

            obj.InvokeEach<Physics>(it => it.SurfaceMap = surfaces);
            var dummy = obj.GetComponents<Dummy>().First();

            obj.Client = client;

            state.Using<IClientController>().AttachCamera(client, obj);

            return obj;
        }

        private IPlayerWeapon BuildWeapon(ChosenWeapon? chosen)
        {
            switch (chosen)
            {
                case ChosenWeapon.Knife:
                    return new KnifeWeapon();
                case ChosenWeapon.Shovel:
                    return new ShowelWeapon();
                case ChosenWeapon.Pickaxe:
                    return new PickaxeWeapon();
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
                    return new DashAbility();
                case ChosenAbility.Cleanse:
                    return new TornadoAbility();
                default:
                    return new DashAbility();
            }
        }
    }
}
