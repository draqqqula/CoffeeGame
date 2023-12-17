using System.Collections;
using Microsoft.Xna.Framework;
using System.Collections.Immutable;
using System.Reflection;
using MagicDustLibrary.ComponentModel;

namespace MagicDustLibrary.Logic
{
    public interface IFamily
    {
        public void AddMember(IControllerProvider state, IFamilyComponent member);

        public void RemoveMember(IControllerProvider state, IFamilyComponent member);

        public void Update(IControllerProvider state, TimeSpan deltaTime);
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
    public abstract class Family<T> : IEnumerable<T>, IFamily where T : IFamilyComponent
    {

        protected List<T> Members { get; } = new List<T>();

        /// <summary>
        /// Вызывается во время <see cref="Game.Update"/>, описывает логику для обновления группы объектов.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="deltaTime"></param>
        protected abstract void CommonUpdate(IControllerProvider state, TimeSpan deltaTime);

        /// <summary>
        /// Вызывается при создании <typeparamref name="T"/> с аттрибутом <see cref="MemberShipAttribute{F}"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="member"></param>
        protected abstract void OnReplenishment(IControllerProvider state, T member);

        /// <summary>
        /// Вызывается при удалении <typeparamref name="T"/>, который является членом семьи.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="member"></param>
        protected abstract void OnAbandonment(IControllerProvider state, T member);

        public void AddMember(IControllerProvider state, IFamilyComponent member)
        {
            if (member is IFamilyComponent)
            {
                AddMember(state, (T)member);
            }
        }

        public void RemoveMember(IControllerProvider state, IFamilyComponent member)
        {
            if (member is IFamilyComponent)
            {
                RemoveMember(state, (T)member);
            }
        }

        private void AddMember(IControllerProvider state, T member)
        {
            Members.Add(member);
            OnReplenishment(state, member);
        }

        private void RemoveMember(IControllerProvider state, T member)
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

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            CommonUpdate(state, deltaTime);
        }
    }

    public interface IFamilyComponent : IComponent
    {
    }
}
