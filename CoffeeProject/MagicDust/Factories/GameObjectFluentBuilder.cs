using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Factorys
{
    public static class GameObjectFluentBuilder
    {
        public static T SetPos<T>(this T obj, Vector2 position) where T : IBodyComponent
        {
            obj.Position = position;
            return obj;
        }

        public static T SetPlacement<T>(this T obj, IPlacement placement) where T : ComponentBase, IDisplayComponent
        {
            var layer = placement.GetLayerType();
            obj.CombineWith(new PlacementInfoComponent() { PlacementInfo = placement, PlacementTarget = obj });
            return obj;
        }

        public static T SetBounds<T>(this T obj, Rectangle bounds) where T : IBodyComponent
        {
            obj.Bounds = bounds;
            return obj;
        }

        public static T AddToState<T>(this T obj, IControllerProvider state) where T : GameObject
        {
            state.Using<IFactoryController>().AddToState(obj);
            return obj;
        }
    }

    internal class PlacementInfoComponent : ComponentBase, IDisposableComponent
    {
        public IDisplayComponent PlacementTarget { get; set; }
        public IPlacement PlacementInfo { get; set; }
        public event OnDispose OnDisposeEvent;

        public void Dispose()
        {
            OnDisposeEvent(this);
        }
    }
}
