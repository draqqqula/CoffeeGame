using CoffeeProject.GameObjects;
using MagicDustLibrary.Logic;

namespace CoffeeProject.Weapons
{
    public interface IPlayerWeapon
    {
        public void UsePrimary(IControllerProvider state, Hero player);
    }
}
