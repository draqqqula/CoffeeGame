using CoffeeProject.Family;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [MemberShip<Enemy>]
    public interface IEnemy : IFamilyComponent
    {
        public int Level { get; set; }
        public event Action<IControllerProvider, GameObject> OnAttack;
        public void SetTarget(IControllerProvider state, GameObject target);
    }

    public static class EnemyExtensions
    {
        public static T SetLevel<T>(this T obj, int level) where T : IEnemy
        {
            obj.Level = level;
            return obj;
        }
    }
}
