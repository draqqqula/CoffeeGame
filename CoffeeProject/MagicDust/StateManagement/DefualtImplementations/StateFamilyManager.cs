﻿using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.DefualtImplementations
{
    public class StateFamilyManager : ComponentHandler<IFamilyComponent>
    {
        private Dictionary<Type, IFamily> _families { get; } = new Dictionary<Type, IFamily>();

        public IEnumerable<IFamily> GetAll()
        {
            return _families.Values;
        }

        public T GetFamily<T>() where T : class, IFamily
        {
            return (T)GetFamily(typeof(T));
        }

        public IFamily GetFamily(Type type)
        {
            if (_families.ContainsKey(type))
            {
                return _families[type];
            }
            var newFamily = Activator.CreateInstance(type) as IFamily;
            _families.Add(type, newFamily);
            return newFamily;
        }

        private static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(Type type)
        {
            var attributeType = typeof(T);
            return type.GetCustomAttributes(attributeType, true)
              .Union(type.GetInterfaces().SelectMany(interfaceType =>
                  interfaceType.GetCustomAttributes(attributeType, true)))
              .Cast<T>();
        }

        private IEnumerable<IFamily> GetFamilies(IFamilyComponent obj)
        {
            var type = obj.GetType();
            var attributes = GetCustomAttributesIncludingBaseInterfaces<IMemberShipContainer>(type);

            if (!attributes.Any())
            {
                return Array.Empty<IFamily>();
            }
            var memberShips = attributes.Where(it => it is IMemberShipContainer);

            if (!memberShips.Any())
            {
                return Array.Empty<IFamily>();
            }

            var families = memberShips.Select(it => GetFamily((it as IMemberShipContainer).FamilyType));

            if (!families.Any())
            {
                return Array.Empty<IFamily>();
            }

            return families;
        }

        public void Introduce(IControllerProvider state, IFamilyComponent obj)
        {
            var families = GetFamilies(obj);
            foreach (var family in families)
            {
                family.AddMember(state, obj);
            }
        }

        public void Abandon(IControllerProvider state, IFamilyComponent obj)
        {
            var families = GetFamilies(obj);
            foreach (var family in families)
            {
                family.RemoveMember(state, obj);
            }
        }

        public override void Hook(IFamilyComponent component)
        {
            Introduce(null, component);
        }

        public override void Unhook(IFamilyComponent component)
        {
            Abandon(null, component);
        }
    }
}
