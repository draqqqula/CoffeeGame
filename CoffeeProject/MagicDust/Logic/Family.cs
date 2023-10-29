using System.Collections;
using Microsoft.Xna.Framework;
using System.Collections.Immutable;
using System.Reflection;

namespace MagicDustLibrary.Logic
{
    public interface IFamily : IStateUpdateable
    {
        public void AddMember(IStateController state, IFamilyMember member);

        public void RemoveMember(IStateController state, IFamilyMember member);
    }
    /// <summary>
    /// Группа объектов с общей логикой обновления.
    /// <list type="table">
    /// <item><typeparamref name="T"/> имеет схожее предзназначение с аналогичным параметром в <see cref="Behavior{T}"/></item>
    /// <item>В семью автоматически добавляются объекты с аттрибутом <see cref="MemberShipAttribute{F}"/>,<br/>
    /// где тип этой семьи указан в качестве параметра</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Family<T> : IEnumerable<T>, IFamily where T : IFamilyMember
    {

        protected List<T> Members { get; } = new List<T>();

        /// <summary>
        /// Вызывается во время <see cref="Game.Update"/>, описывает логику для обновления группы объектов.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="deltaTime"></param>
        protected abstract void CommonUpdate(IStateController state, TimeSpan deltaTime);

        /// <summary>
        /// Вызывается при создании <typeparamref name="T"/> с аттрибутом <see cref="MemberShipAttribute{F}"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="member"></param>
        protected abstract void OnReplenishment(IStateController state, T member);

        /// <summary>
        /// Вызывается при удалении <typeparamref name="T"/>, который является членом семьи.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="member"></param>
        protected abstract void OnAbandonment(IStateController state, T member);

        public void AddMember(IStateController state, IFamilyMember member)
        {
            if (member is IFamilyMember)
            {
                AddMember(state, (T)member);
            }
        }

        public void RemoveMember(IStateController state, IFamilyMember member)
        {
            if (member is IFamilyMember)
            {
                RemoveMember(state, (T)member);
            }
        }

        private void AddMember(IStateController state, T member)
        {
            Members.Add(member);
            OnReplenishment(state, member);
        }

        private void RemoveMember(IStateController state, T member)
        {
            Members.Remove(member);
            OnAbandonment(state, member);
        }

        public IEnumerable<T> GetSortedLazy(Func<T, IComparable> keySelector)
        {
            return Members.OrderBy(keySelector);
        }

        public IEnumerable<T> GetSorted(IComparer<T> comparer)
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

        public void Update(IStateController state, TimeSpan deltaTime)
        {
            CommonUpdate(state, deltaTime);
        }
    }

    public interface IFamilyMember
    {
    }
}
