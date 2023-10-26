using MagicDustLibrary.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicDustLibrary.Animations
{
    public class AnimationFrame
    {
        /// <summary>
        /// область на общем изображении
        /// </summary>
        private Rectangle Borders;
        /// <summary>
        /// точка которая будет совпадать с позицией объекта
        /// </summary>
        private Vector2 Anchor;
        /// <summary>
        /// длительность кадра
        /// </summary>
        public TimeSpan Duration;

        public AnimationFrame(Rectangle borders, Vector2 anchor, TimeSpan duration)
        {
            Borders = borders;
            Anchor = anchor;
            Duration = duration;
        }

        public AnimationFrame(int x, int y, int width, int height, int x0, int y0, double duration) :
            this(new Rectangle(x, y, width, height), new Vector2(x0, y0), TimeSpan.FromSeconds(duration))
        {
        }

        /// <summary>
        /// отправляет кадр в буфер отрисовки
        /// </summary>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="sheet"></param>
        /// <param newPriority="animator"></param>
        public IDisplayable CreateDrawable(DrawingParameters arguments, Texture2D sheet)
        {
            return new FrameForm(Borders, Anchor, arguments, sheet);
        }
    }
}
