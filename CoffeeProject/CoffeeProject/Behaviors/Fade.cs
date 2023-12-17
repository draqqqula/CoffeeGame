using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Behaviors
{
    public class Fade : Behavior<GameObject>
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

        protected override void Update(IControllerProvider state, TimeSpan deltaTime, GameObject parent)
        {
            t += deltaTime;
            if (DestroyAfterFadeOut && CurrentOpacity == 0)
                parent.Dispose();
        }

        protected override DrawingParameters ChangeAppearance(GameObject parent, DrawingParameters parameters)
        {
            parameters.Color *= CurrentOpacity;
            return parameters;
        }

        public Fade(TimeSpan delay, TimeSpan fadeDuration, TimeSpan t0, bool destroyAfterFadeOut, bool enabled) : base()
        {
            Delay = delay;
            FadeDuration = fadeDuration;
            t = t0;
            Enabled = enabled;
            DestroyAfterFadeOut = destroyAfterFadeOut;
        }
    }
}
