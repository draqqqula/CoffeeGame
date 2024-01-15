using BehaviorKit;
using CoffeeProject.Elements;
using CoffeeProject.GameObjects;
using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic.Controllers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.CustomNodes
{
    public class ElementFilter : NodeComponent, IDisplayFilter
    {
        private readonly Color _filterColor;
        public ElementFilter(DamageType element)
        {
            switch(element)
            {
                case DamageType.Fire: _filterColor = Color.PeachPuff; break;
                case DamageType.Ice: _filterColor = Color.SkyBlue; break;
                case DamageType.Light: _filterColor = Color.LightYellow; break;
                case DamageType.Holy: _filterColor= Color.Yellow; break;
                case DamageType.Dark: _filterColor = Color.MediumPurple; break;
                default: _filterColor = Color.White; break;
            }
        }
        public DrawingParameters ApplyFilter(DrawingParameters info)
        {
            return info with { Color = _filterColor };
        }
    }

    public static class ElementExtensions
    {
        private static readonly Dictionary<DamageType, double> ElementChances = 
            new Dictionary<DamageType, double> 
            { 
                { DamageType.Fire, 0.1 }, 
                { DamageType.Light, 0.1 },
                { DamageType.Dark, 0.1 },
                { DamageType.Ice, 0.1 }
            };

        public static DamageType? GetRandomDamageType()
        {
            var random = new Random();
            double randomValue = random.NextDouble();

            var counter = 0.0;
            foreach (var kvp in ElementChances)
            {
                counter += kvp.Value;
                if (randomValue <= counter)
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        private static readonly Dictionary<DamageType, DamageType> ElementResistances =
            new Dictionary<DamageType, DamageType>
            {
                { DamageType.Fire, DamageType.Ice },
                { DamageType.Light, DamageType.Dark },
                { DamageType.Dark, DamageType.Light },
                { DamageType.Ice, DamageType.Fire }
            };

        public static T RandomizeElement<T>(this T enemy) where T : GameObject, IEnemy
        {
            var element = GetRandomDamageType();
            if (element is null)
            {
                return enemy;
            }

            var dummy = enemy.GetComponents<Dummy>().First();
            foreach (var e in ElementResistances.Keys.Except([ElementResistances[element.Value]]))
            {
                dummy.Resistances.Add(e, 1);
            }
            enemy.AddComponent(new ElementFilter(element.Value));

            if (element == DamageType.Fire)
            {
                enemy.CombineWith(new FireSpawner());
            }
            else if (element == DamageType.Ice)
            {
                enemy.CombineWith(new IceSpawner());
            }
            else if (element == DamageType.Light)
            {
                enemy.CombineWith(new HolySpawner());
            }
            else if (element == DamageType.Dark)
            {
                enemy.Level += 1;
            }

            return enemy;
        }
    }
}
