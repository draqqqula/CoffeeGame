using MagicDustLibrary.Common;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.DefualtImplementations
{
    public class StateLayerManager : ComponentHandler<PlacementInfoComponent>
    {
        private readonly List<Layer> LayerOrder = new();
        private readonly Dictionary<Type, Layer> LayerTypes = new();
        private readonly Dictionary<IDisplayComponent, Layer> Placements = new();


        public Layer? GetLayer(IDisplayComponent component)
        {
            if (Placements.TryGetValue(component, out Layer layer))
            {
                return layer;
            }
            return null;
        }

        public IPlacement? GetPlacement(IDisplayComponent component)
        {
            var layer = GetLayer(component);
            if (layer is not null)
            {
                return new Placement(layer.GetType());
            }
            return null;
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

        public override void Hook(PlacementInfoComponent component)
        {
            var placement = component.PlacementInfo;
            var layer = GetLayer(placement.GetLayerType());
            var target = component.PlacementTarget;
            Placements.Add(target, layer);
            layer.PlaceTop(target);
        }

        public override void Unhook(PlacementInfoComponent component)
        {
            var target = component.PlacementTarget;
            GetLayer(target)?.Remove(target);
            if (Placements.ContainsKey(target))
            {
                Placements.Remove(target);
            }
        }
    }
}
