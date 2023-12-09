using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Logic
{
    /// <summary>
    /// Позволяет получить доступ к контроллерам состояния <see cref="IUpdateComponent.Update(IControllerProvider, TimeSpan)"/>.
    /// </summary>
    public interface IControllerProvider
    {
        public T Using<T>() where T : IStateController;
    }

    public interface IStateController
    {
    }
}
