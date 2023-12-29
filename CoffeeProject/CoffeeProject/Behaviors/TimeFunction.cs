using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.Behaviors
{
    public class TimeFunction : Behavior<IMultiBehaviorComponent>
    {
        protected double t = 0;
        protected Action<double> Action { get; set; }
        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            Action.Invoke(t);
            t += deltaTime.TotalSeconds;
        }

        public TimeFunction(Action<double> action)
        {
            Action = action;
        }
    }

    public class SizeSine<T> : Behavior<T>, IDisplayFilter where T : class, IMultiBehaviorComponent, IBodyComponent
    {
        protected float t = 0;
        public float Amplitude { get; set; } = 1;
        public float Factor { get; set; } = 1;

        public SizeSine(float amplitude, float factor)
        {
            Amplitude = amplitude;
            Factor = factor;
        }

        public DrawingParameters ApplyFilter(DrawingParameters info)
        {
            info.Scale += new Vector2(MathF.Sin(t * Factor) * Amplitude);
            return info;
        }

        protected override void Act(IControllerProvider state, TimeSpan deltaTime, T parent)
        {
            t += Convert.ToSingle(deltaTime.TotalSeconds);
        }
    }
}
