using CoffeeProject.Layers;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    public class Shadow : NodeComponent, IDisplayComponent
    {
        private IBodyComponent _box;
        private Texture2D _texture;
        private readonly IContentStorage _contentStorage;
        public Shadow(IContentStorage content)
        {
            AddGreetingFor<IBodyComponent>(it =>
            {
                if (_box is null)
                {
                    _box = it;
                }
            }
            );
            _contentStorage = content;

            _texture = _contentStorage.GetAsset<Texture2D>("shadow");
        }

        public event OnDispose OnDisposeEvent = delegate { };

        public void Dispose()
        {
            OnDisposeEvent(this);
        }

        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            yield return new FrameForm(_texture.Bounds, _texture.Bounds.Location.ToVector2(), layer.Process(GetDrawingParameters(), camera), _texture);
        }

        public DrawingParameters GetDrawingParameters()
        {
            var layout = _box.GetLayout();
            var info = new DrawingParameters()
            {
                Position = layout.Location.ToVector2() + new Vector2(-_box.Bounds.Width * 0.25f, _box.Bounds.Height * 0.5f),
                Scale = (_box.Bounds.Size.ToVector2() / _texture.Bounds.Size.ToVector2()) * new Vector2(1.5f, 1)
            };
            return info;
        }
    }

    public static class ShadowExtensions
    {
        public static T AddShadow<T>(this T obj, IControllerProvider state) where T : GameObject, IBodyComponent
        {
            obj.CombineWith(
                state.Using<IFactoryController>()
                .CreateObject<Shadow>()
                .SetPlacement(new Placement<ShadowLayer>()));
            return obj;
        }
    }
}
