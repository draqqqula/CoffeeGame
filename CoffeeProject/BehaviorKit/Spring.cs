using MagicDustLibrary.Display;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization.BaseServices;
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

        public Spring(float subsidence)
        {
            this._subsidence = subsidence;
        }

        public void Pull(float factor)
        {
            _factor = factor;
        }

        protected override void Act(IStateController state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            if (_factor > 1)
            {
                _factor = MathF.Max(1, MathEx.Lerp(_factor, 1, _subsidence, deltaTime));
            }
        }

        public DrawingParameters ApplyFilter(DrawingParameters info)
        {
            if (_factor > 1)
            {
                info.Scale = new Vector2(info.Scale.X, info.Scale.Y * _factor);
            }
            return info;
        }
    }
}
