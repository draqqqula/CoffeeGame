using MagicDustLibrary.Display;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    public interface IDisplayComponent : IDisposableComponent
    {
        /// <summary>
        /// Возвращает несколько объектов для отрисовки.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer);
        public DrawingParameters GetDrawingParameters();
        public Type GetLayerType();
    }
}
