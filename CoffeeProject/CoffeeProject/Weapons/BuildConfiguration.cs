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
        [PartDescription("кинжалом")][NamePart("Ни")] Knife,
        [PartDescription("секирой")][NamePart("Ар")] Axe,
        [PartDescription("мечом")][NamePart("Па")] Sword,
        [PartDescription("луком")][NamePart("Ло")] Bow
    }

    public enum ChosenElement
    {
        [PartDescription("огнем")][NamePart("тем")] Fire,
        [PartDescription("льдом")][NamePart("ко")] Ice,
        [PartDescription("молнией")][NamePart("сен")] Lightning
    }

    public enum ChosenAbility
    {
        [PartDescription("рывок")][NamePart("ий")] Dash,
        [PartDescription("остановку времени")][NamePart("лай")] Block,
        [PartDescription("защитное поле")][NamePart("ля")] Cleanse
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

        public string GetDescription()
        {
            return $"Владеет {BuildHelper.GetDescription(Weapon)}, " +
                $"\nуправляет {BuildHelper.GetDescription(Element)}, " +
                $"\nспособен делать {BuildHelper.GetDescription(Ability)}";
        }
    };

    [AttributeUsage(AttributeTargets.Field)]
    public class NamePartAttribute(string part) : Attribute
    {
        public string Part = part;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PartDescriptionAttribute(string text) : Attribute
    {
        public string Text = text;
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

        public static string GetDescription(Enum enumValue)
        {
            if (enumValue is null)
            {
                return "...";
            }
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString());
            var member = memberInfo.FirstOrDefault(m => m.DeclaringType == type);
            var attribute = member.GetCustomAttribute<PartDescriptionAttribute>();
            return attribute.Text ?? "...";
        }
    }
}
