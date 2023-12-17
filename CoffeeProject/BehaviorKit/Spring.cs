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
    public class Spring : Behavior<IMultiBehaviorComponent>, IDisplayFilter
    {
        private float _factor = 1;
        private float _subsidence;

        public DrawingParameters ApplyFilter(DrawingParameters parameters)
        {
            if (_factor > 1)
            {
                parameters.Scale = new Vector2(parameters.Scale.X, parameters.Scale.Y * _factor);
            }
            return parameters;
        }

        public void Pull(float factor)
        {
            _factor = factor;
        }

        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            if (_factor > 1)
            {
                _factor = MathF.Max(1, MathEx.Lerp(_factor, 1, _subsidence, deltaTime));
            }
        }

        public Spring(float subsidence)
        {
            this._subsidence = subsidence;
        }
    }
}
