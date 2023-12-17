using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Behaviors
{
    public class Fade : Behavior<IMultiBehaviorComponent>, IDisplayFilter
    {

        public TimeSpan Delay { get; private set; }
        public TimeSpan FadeDuration { get; private set; }
        public TimeSpan t { get; private set; }
        public bool DestroyAfterFadeOut { get; private set; }
        public float CurrentOpacity
        {
            get
            {
                return (float)(1 - Math.Min((Math.Max((t - Delay).TotalSeconds, 0) / FadeDuration.TotalSeconds), 1));
            }
        }

        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            t += deltaTime;
            if (DestroyAfterFadeOut && CurrentOpacity == 0)
                parent.Dispose();
        }

        public DrawingParameters ApplyFilter(DrawingParameters parameters)
        {
            parameters.Color *= CurrentOpacity;
            return parameters;
        }

        public Fade(TimeSpan delay, TimeSpan fadeDuration, TimeSpan t0, bool destroyAfterFadeOut) : base()
        {
            Delay = delay;
            FadeDuration = fadeDuration;
            t = t0;
            DestroyAfterFadeOut = destroyAfterFadeOut;
        }
    }
}
