using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization.Services
{
    public interface IDisplayService
    {
        public void Draw(GameClient client, SpriteBatch batch);
    }
}
