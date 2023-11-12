using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorKit
{
    public class Spring : Behavior<IMultiBehaviorComponent>
    {
        private float _factor = 1;
        private float _subsidence;

        protected override DrawingParameters ChangeAppearance(IMultiBehaviorComponent parent, DrawingParameters parameters)
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

        protected override void Update(IStateController state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
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
