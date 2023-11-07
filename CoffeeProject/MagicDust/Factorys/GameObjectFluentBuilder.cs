using MagicDustLibrary.Logic;
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
        public static T SetPos<T>(this T obj, Vector2 position) where T : GameObject
        {
            obj.SetPosition(position);
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

        public static T SetBounds<T>(this T obj, Rectangle bounds) where T : GameObject
        {
            obj.SetBounds(bounds);
            return obj;
        }

        public static T AddToState<T>(this T obj, IStateController state) where T : GameObject
        {
            state.AddToState(obj);
            return obj;
        }
    }
}
