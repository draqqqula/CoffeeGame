using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.BaseServices;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public static T SetPlacement<T>(this T obj, IPlacement placement) where T : GameObject
        {
            obj.Placement = placement;
            return obj;
        }

        public static GameObject PlaceTo<L>(this GameObject obj) where L : Layer
        {
            obj.Placement = new Placement<L>();
            return obj;
        }

        public static T SetBounds<T>(this T obj, Rectangle bounds) where T : IBodyComponent
        {
            obj.SetBounds(bounds);
            return obj;
        }

        public static T AddToState<T>(this T obj, IStateController state) where T : IDisposableComponent
        {
            state.AddToState(obj);
            return obj;
        }
    }

    public record struct GameObjectBuilderContext<T> where T : IComponent
    {
        private Dictionary<string, object> _creationEntries;
        private ComponentShell _shell;
        public T Target;

        public static implicit operator T(GameObjectBuilderContext<T> context)
        {
            return context.Target;
        }

        private class ContextEntryContainer
        {

        }
    }

    public interface IContextEntry
    {
    }
}
