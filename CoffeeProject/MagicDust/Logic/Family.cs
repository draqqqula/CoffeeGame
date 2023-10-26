using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace MagicDustLibrary.Logic
{
    public interface IFamily : IStateUpdateable
    {
        public virtual void CommonUpdate(TimeSpan deltaTime)
        {
        }

        public virtual void Initialize(IFamilyMember member)
        {
        }

        public virtual void OnReplenishment(IFamilyMember member)
        {
        }

        public virtual void OnAbandonment(IFamilyMember member)
        {
        }
    }
    public abstract class Family<T> : IEnumerable<T> where T : IFamilyMember
    {

        protected List<T> Members = new List<T>();

        public virtual void CommonUpdate(TimeSpan deltaTime)
        {
        }

        public virtual void Initialize(T member)
        {
        }

        public virtual void OnReplenishment(T member)
        {
        }

        public virtual void OnAbandonment(T member)
        {
        }

        public void AddMember(T member)
        {
            Members.Add(member);
            OnReplenishment(member);
        }

        public void RemoveMember(T member)
        {
            Members.Remove(member);
            OnAbandonment(member);
        }

        public IEnumerable<T> PickLazy(Func<T, IComparable> keySelector)
        {
            return Members.OrderBy(keySelector);
        }

        public IEnumerable<T> Pick(IComparer<T> comparer)
        {
            var newCollection = Members.ToList();
            newCollection.Sort(comparer);
            return newCollection;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Members.GetEnumerator();
        }
    }

    public interface IFamilyMember
    {
    }
}
