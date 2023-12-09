using Microsoft.Xna.Framework;


namespace MagicDustLibrary.Logic
{
    public interface IUpdateComponent : IDisposableComponent
    {
        /// <summary>
        /// Вызвается у объектов во время <see cref="Game.Update"/>.<br/>
        /// Код, выполняемый объектом <b>каждый кадр</b>.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="deltaTime"></param>
        public void Update(IControllerProvider state, TimeSpan deltaTime);
    }
}
