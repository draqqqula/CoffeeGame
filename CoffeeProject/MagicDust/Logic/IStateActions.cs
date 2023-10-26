using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IStateController
    {
        public T CreateObject<T, L>(Vector2 position) where T : GameObject where L : Layer;
        public void PlaceAbove(GameObject target, GameObject source);
        public void PlaceBelow(GameObject target, GameObject source);
        public void PlaceTop(GameObject target);
        public void PlaceBottom(GameObject target);
        public void PlaceTo<L>(GameObject target) where L : Layer;
        public void GetFamily<F>() where F : class, IFamily;
        public void OpenServer(int port);
    }
}
