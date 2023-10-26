using MagicDustLibrary.Logic;

namespace MagicDustLibrary.Organization
{
    public class StateUpdateManager : IStateUpdateable
    {
        private readonly List<IStateUpdateable> Updateables = new List<IStateUpdateable>();

        public void AddUpdateable(IStateUpdateable obj)
        {
            Updateables.Add(obj);
        }

        public void RemoveUpdateable(IStateUpdateable obj)
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