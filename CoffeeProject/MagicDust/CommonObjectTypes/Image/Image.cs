using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.CommonObjectTypes.Image
{
    public class Image : GameObject, IDisplayComponent, IBodyComponent, IMultiBehaviorComponent, IUpdateComponent
    {
        private readonly GraphicsDevice _device;
        private readonly IContentStorage _contentStorage;

        public Image(MagicGameApplication app, IContentStorage storage)
        {
            _device = app.Services.GetService<Game>().GraphicsDevice;
            _contentStorage = storage;
        }

        private Texture2D? Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
        public Vector2 Scale { get; set; }
        public float Opacity { get; set; } = 1f;
        public Color Color { get; set; } = Color.White;

        public event Action<IControllerProvider, TimeSpan, IMultiBehaviorComponent> OnAct = delegate { };

        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            if (Texture is null)
            {
                yield break;
            }
            yield return new FrameForm(
                Texture.Bounds, 
                Texture.Bounds.Center.ToVector2(), 
                layer.Process(GetDrawingParameters(), camera), 
                Texture);
        }

        public Image SetTexture(Texture2D texture)
        {
            Texture = texture;
            return this;
        }

        public Image SetTexture(string path)
        {
            return SetTexture(_contentStorage.GetAsset<Texture2D>(path));
        }

        public Image SetScale(Vector2 scale)
        {
            Scale = scale;
            return this;
        }

        public Image SetOpacity(float opacity)
        {
            Opacity = opacity;
            return this;
        }

        public Image SetMonoColor(Color color)
        {
            var texture = new Texture2D(_device, 1, 1);
            texture.SetData(new Color[] { color });
            return SetTexture(texture);
        }

        public DrawingParameters GetDrawingParameters()
        {
            var info = new DrawingParameters()
            {
                Position = Position,
                Scale = Scale,
                Color = Color * Opacity
            };
            foreach (var filter in GetComponents<IDisplayFilter>())
            {
                info = filter.ApplyFilter(info);
            }
            return info;
        }

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            OnAct(state, deltaTime, this);
        }
    }
}
