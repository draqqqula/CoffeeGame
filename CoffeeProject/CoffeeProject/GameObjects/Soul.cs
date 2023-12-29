using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    [SpriteSheet("soul")]
    internal class Soul : Sprite, IMultiBehaviorComponent, IUpdateComponent
    {
        public Soul(IAnimationProvider provider) : base(provider)
        {
            
        }

        public float Scale { get; set; } = 1f;
        public float Opacity { get; set; } = 1f;
        protected override DrawingParameters DisplayInfo
        {
            get
            {
                var info = base.DisplayInfo;
                info.Scale = new Vector2(Scale * base.DisplayInfo.Scale.X, Scale * base.DisplayInfo.Scale.X);
                info.Color = info.Color * Opacity;
                info.Priority = new Random().NextSingle();
                return info;
            }
        }

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct;

        public Soul SetScale(float scale)
        {
            Scale = scale;
            return this;
        }

        public Soul SetOpacity(float opacity)
        {
            Opacity = opacity;
            return this;
        }

        public override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            base.Update(state, deltaTime);
            OnAct(state, deltaTime, this);
        }
    }
}
