using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Weapons
{
    public enum ChosenWeapon
    {
        [NamePart("Ни")] Knife,
        [NamePart("Ар")] Shovel,
        [NamePart("Па")] Pickaxe
    }

    public enum ChosenElement
    {
        [NamePart("тем")] Fire,
        [NamePart("ко")] Ice,
        [NamePart("сен")] Lightning
    }

    public enum ChosenAbility
    {
        [NamePart("ий")] Dash,
        [NamePart("лай")] Block,
        [NamePart("ля")] Cleanse
    }

    public record struct BuildConfiguration(ChosenWeapon? Weapon, ChosenElement? Element, ChosenAbility? Ability)
    {
        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Weapon.ToString();
                    case 1:
                        return Element.ToString();
                    case 2:
                        return Ability.ToString();
                    default:
                        return null;
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        Weapon = BuildHelper.FromNamePart<ChosenWeapon>(value);
                        break;
                    case 1:
                        Element = BuildHelper.FromNamePart<ChosenElement>(value);
                        break;
                    case 2:
                        Ability = BuildHelper.FromNamePart<ChosenAbility>(value);
                        break;
                }
            }
        }
    };

    [AttributeUsage(AttributeTargets.Field)]
    public class NamePartAttribute(string part) : Attribute
    {
        public string Part = part;
    }

    public static class BuildHelper
    {
        public static T FromNamePart<T>(string namePart) where T : Enum
        {
            var enumType = typeof(T);

            var matchingField = enumType.GetFields()
                .FirstOrDefault(field =>
                {
                    var attribute = field.GetCustomAttribute<NamePartAttribute>();
                    return attribute != null && attribute.Part == namePart;
                });

            if (matchingField != null)
            {
                return (T)matchingField.GetValue(null);
            }

            throw new ArgumentException($"No enum value with NamePartAttribute matching '{namePart}' found.");
        }
    }
}
