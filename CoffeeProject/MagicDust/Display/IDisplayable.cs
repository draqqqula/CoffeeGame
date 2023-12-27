using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Display
{

    /// <summary>
    /// Объект, предназначенный для помещения в буфер отрисовки клиента <see cref="ViewBuffer"/>
    /// </summary>
    public interface IDisplayable
    {
        /// <summary>
        /// Отрисовывает объект на экране.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="camera"></param>
        /// <param name="contentStorage"></param>
        public void Draw(SpriteBatch batch, GameCamera camera, IContentStorage contentStorage);

        public IComparable OrderComparer { get; }
    }
}
