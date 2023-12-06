using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StateLayerManager : ComponentHandler<IDisplayComponent>
    {
        private readonly List<Layer> LayerOrder = new();
        private readonly Dictionary<Type, Layer> LayerTypes = new();

        public StateLayerManager() {
            var a = 1;
        }

        public T GetLayer<T>() where T : Layer
        {
            if (LayerOrder.Any(it => it is T))
                return LayerOrder.Where(it => it is T).First() as T;
            return CreateLayer<T>();
        }

        public Layer GetLayer(Type type)
        {
            if (!LayerTypes.ContainsKey(type))
                return CreateLayer(type);
            return LayerTypes[type];
        }

        private Layer CreateLayer(Type type)
        {
            var newLayer = Activator.CreateInstance(type) as Layer;
            LayerOrder.Add(newLayer);
            LayerOrder.Sort(new LayerComparer());
            LayerTypes.Add(type, newLayer);
            return newLayer;
        }

        private T CreateLayer<T>() where T : Layer
        {
            var newLayer = Activator.CreateInstance<T>();
            LayerOrder.Add(newLayer);
            LayerOrder.Sort(new LayerComparer());
            LayerTypes.Add(typeof(T), newLayer);
            return newLayer;
        }

        public IEnumerable<Layer> GetAll()
        {
            return LayerOrder.ToArray();
        }

        public override void Hook(IDisplayComponent component)
        {
            GetLayer(component.GetLayerType()).PlaceTop(component);
        }

        public override void Unhook(IDisplayComponent component)
        {
            GetLayer(component.GetLayerType()).Remove(component);
        }
    }
}
