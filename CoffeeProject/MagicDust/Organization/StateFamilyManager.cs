using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StateFamilyManager
    {
        private ImmutableDictionary<Type, IFamily> Families { get; init; } =
        Assembly.GetEntryAssembly()
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Family<>)))
            .ToDictionary(it => it, it => it.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IFamily)
            .ToImmutableDictionary();

        public T GetFamily<T>() where T : class, IFamily
        {
            return Families[typeof(T)] as T;
        }

        public IFamily GetFamily(Type type)
        {
            return Families[type] as IFamily;
        }
    }
}
