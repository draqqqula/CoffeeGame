using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;

namespace MagicDustLibrary.Organization.DefualtImplementations
{
    public class StateUpdateManager : ComponentHandler<IUpdateComponent>, IUpdateService
    {
        private readonly List<IUpdateComponent> Updateables = new List<IUpdateComponent>();
        public Dictionary<IUpdateComponent, double> TimeScales { get; set; } = [];

        public bool RunOnPause => false;

        public override void Hook(IUpdateComponent component)
        {
            Updateables.Add(component);
        }

        public override void Unhook(IUpdateComponent component)
        {
            Updateables.Remove(component);
            if (TimeScales.ContainsKey(component))
            {
                TimeScales.Remove(component);
            }
        }

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            var collection = Updateables.ToArray();
            foreach (var updateable in collection)
            {
                if (TimeScales.TryGetValue(updateable, out var scale))
                {
                    updateable.Update(state, deltaTime * scale);
                    continue;
                }
                updateable.Update(state, deltaTime);
            }
        }
    }
}