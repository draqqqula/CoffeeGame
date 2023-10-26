using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.Organization;

namespace MagicDustLibrary.Logic
{
    public interface IPlacement
    {
        Type GetLayerType();
    }
    public class Placement<T> : IPlacement where T : Layer
    {
        public Type GetLayerType()
        {
            return typeof(T);
        }

        public static Placement<T> On<T>() where T : Layer
        {
            return new Placement<T>();
        }
    }
}