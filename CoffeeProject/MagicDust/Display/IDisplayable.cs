using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Display
{
    public interface IDisplayable
    {
        public void Draw(SpriteBatch batch, GameCamera camera, IContentStorage contentStorage);
    }
}
