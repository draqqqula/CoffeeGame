using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.DefualtImplementations;
using MagicDustLibrary.Organization.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.BoxDisplay
{
    /// <summary>
    /// Отображает хитбокс обекта, к которому добавлен
    /// </summary>
    public class BoxDisplay : NodeComponent, IDisplayComponent
    {
        private IBodyComponent _box;
        private readonly Texture2D _texture;
        public BoxDisplay(IContentStorage content)
        {
            AddGreetingFor<IBodyComponent>(it => _box = it);

            if (content is DefaultContentStorage contentStorage)
            {
                _texture = contentStorage.GenerateMissingTexture(Color.Black, Color.Purple, 3);
            }
        }

        public event OnDispose OnDisposeEvent;

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
            var info = new DrawingParameters()
            {
                Position = _box.Position + _box.Bounds.Location.ToVector2(),
                Scale = _box.Bounds.Size.ToVector2()/_texture.Bounds.Size.ToVector2()
            };
            return info;
        }
    }
}
