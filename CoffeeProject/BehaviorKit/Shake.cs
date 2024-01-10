using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorKit
{
    public class ShakeInstance
    {
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public double t { get; set; } = 0;
        public float Amplitude { get; set; }
        public float Absorption { get; set; }
        public double Interval { get; set; }
        public double Gap { get; set; }
        public int Count { get; set; }
        public bool OnGap { get; set; } = false;

        public event OnDispose OnDisposeEvent;

        public bool Continue(TimeSpan deltaTime, ref Vector2 vector)
        {
            t += deltaTime.TotalSeconds;

            var elapsedCount = Convert.ToInt32(Math.Floor(t / Interval));
            var interationTime = t % Interval;

            if (interationTime >= Gap && !OnGap)
            {
                Amplitude -= Absorption;
                OnGap = true;
                Offset = Vector2.Zero;
            }

            if (interationTime < Gap && OnGap)
            {
                var random = new RandomEx();
                OnGap = false;
                Offset = MathEx.AngleToVector(random.NextSingle(0, 2 * MathF.PI, 1f)) * Amplitude;
            }

            vector += Offset;

            return elapsedCount < Count;
        }
    }

    public class VisualShake : Behavior<IMultiBehaviorComponent>, IDisplayFilter
    {
        private IEnumerable<ShakeInstance> _instances = [];
        private Vector2 Offset { get; set; } = Vector2.Zero;
        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            var offset = Vector2.Zero;
            _instances = _instances.Where(i => i.Continue(deltaTime, ref offset)).ToArray();
            Offset = offset;
        }

        public void Start(float amplitude, TimeSpan interval, TimeSpan gap, int count, float absorption)
        {
            var shake = new ShakeInstance()
            {
                Amplitude = amplitude,
                Absorption = absorption,
                Interval = interval.TotalSeconds,
                Gap = gap.TotalSeconds,
                Count = count
            };
            _instances = _instances.Append(shake);
        }

        DrawingParameters IDisplayFilter.ApplyFilter(DrawingParameters info)
        {
            info.Position += Offset;
            return info;
        }
    }
}
