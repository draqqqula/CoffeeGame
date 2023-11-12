using MagicDustLibrary.Logic;

namespace MagicDustLibrary.Organization
{
    public class StateUpdateManager
    {
        private readonly List<IUpdateComponent> Updateables = new List<IUpdateComponent>();

        public void AddUpdateable(IUpdateComponent obj)
        {
            Updateables.Add(obj);
        }

        public void RemoveUpdateable(IUpdateComponent obj)
        {
            Updateables.Remove(obj);
        }

        public void Update(IStateController state, TimeSpan deltaTime)
        {
            var collection = Updateables.ToArray();
            foreach (var updateable in collection)
            {
                updateable.Update(state, deltaTime);
            }
        }
    }
}