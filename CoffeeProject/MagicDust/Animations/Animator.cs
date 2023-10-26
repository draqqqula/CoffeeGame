using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Animations
{
    public class Animator
    {
        private readonly Dictionary<string, Animation> Animations;
        public TimeSpan RunDuration { get; private set; }
        /// <summary>
        /// текущая анимация
        /// </summary>
        public Animation Running { get; private set; }
        public bool OnPause { get; private set; }

        /// <summary>
        /// проигрывает анимацию далее
        /// по завершении анимации переключает на следующую если та задана
        /// если же следующая анимация не задана а у текущей Looping == true проигрывает текущую анимацию с начала
        /// в остальных случаях при завершении начинает проигрывание анимации Default
        /// </summary>
        /// <param newPriority="arguments"></param>
        public void Update(TimeSpan deltaTime)
        {
            if (!OnPause)
            {
                RunDuration += deltaTime;
            }
            if (!Running.Run(RunDuration) && !OnPause)
            {
                if (Running.NextAnimation != null)
                    ChangeAnimation(Running.NextAnimation, 0);

                else if (Running.Looping)
                    ChangeAnimation(Running.Name, 0);

                else
                    ChangeAnimation("Default", 0);
            }
        }

        /// <summary>
        /// продолжает проигрывание текущей анимации с этого кадра
        /// </summary>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="frame"></param>
        public void SetFrame(DrawingParameters arguments, int frame)
        {
            ChangeAnimation(Running.Name, frame);
        }

        public void Stop() => OnPause = true;
        public void Resume() => OnPause = false;
        public void TogglePause() => OnPause = !OnPause;

        /// <summary>
        /// переключает анимацию и начинает проигрывание с этого кадра
        /// </summary>
        /// <param newPriority="arguments"></param>
        /// <param newPriority="animation"></param>
        /// <param newPriority="initialFrame"></param>
        public void ChangeAnimation(string animation, int initialFrame)
        {
            Running = Animations[animation];
            RunDuration = Running.Duration * Running.SpeedFactor * (initialFrame / (double)Running.FrameCount);
            Resume();
            Update(TimeSpan.Zero);
        }

        public void SetAnimation(string animation, int initialFrame)
        {
            if (animation != Running.Name)
                ChangeAnimation(animation, initialFrame);
        }

        internal IDisplayable GetVisual(DrawingParameters arguments)
        {
            return Running.GetVisual(arguments);
        }

        public Animator(Dictionary<string, Animation> animations, string initial)
        {
            if (animations.ContainsKey("Default"))
            {
                OnPause = false;
                Animations = animations;
                Running = Animations[initial];
                RunDuration = TimeSpan.Zero;
            }
            else
            {
                throw new FormatException("\"Default\" animation not found");
            }
        }

        public Animator(string animationsFile, string initial, IAnimationProvider animationBuilder) :
            this(animationBuilder.BuildFromFiles(animationsFile), initial)
        {
        }
    }
}
