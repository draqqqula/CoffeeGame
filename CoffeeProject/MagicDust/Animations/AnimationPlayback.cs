using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Animations
{
    public class AnimationPlayback(Animation animation)
    {
        public int CurrentFrame { get; private set; } = 0;
        public Animation TargetAnimation { get; init; } = animation;

        /// <summary>
        /// если возможно, выбирает кадр отталкиваясь от прогресса анимации, отправляет в буфер на отрисовку и возвращает true
        /// если невозможно выбрать кадр, возвращает false
        /// </summary>
        /// <param newPriority="progress"></param>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="animator"></param>
        /// <returns></returns>
        public bool Run(double progress)
        {
            if (progress > 1 || progress < 0)
            {
                return false;
            }

            CurrentFrame = Math.Min((int)Math.Floor(progress * TargetAnimation.FrameCount), TargetAnimation.FrameCount - 1);
            return true;
        }

        /// <summary>
        /// если возможно, выбирает кадр отталкиваясь от длительности анимации, отправляет в буфер на отрисовку и возвращает true
        /// если невозможно выбрать кадр, возвращает false
        /// </summary>
        /// <param newPriority="t"></param>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="animator"></param>
        /// <returns></returns>
        public bool Run(TimeSpan t)
        {
            return Run(t / TargetAnimation.SpeedFactor / TargetAnimation.Duration);
        }

        public IDisplayable GetVisual(DrawingParameters arguments)
        {
            return TargetAnimation.GetVisual(arguments, CurrentFrame);
        }
    }
}
