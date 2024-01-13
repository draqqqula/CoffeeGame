using MagicDustLibrary.Animations;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace MagicDustLibrary.CommonObjectTypes
{
    /// <summary>
    /// Специальный тип <see cref="GameObject"/>, поддерживающий анимации.
    /// </summary>
    public abstract class Sprite : GameObject, IDisplayComponent, IBodyComponent, IUpdateComponent
    {
        public bool IsMirroredVertical = false;
        public bool IsMirroredHorizontal = false;
        public bool Invisible { get; set; } = false;

        public event OnDispose OnDisposeEvent;

        protected Sprite(IAnimationProvider provider) : base()
        {
            var attribute = GetType().GetCustomAttribute<SpriteSheetAttribute>();
            if (attribute != null)
            {
                Animator = new Animator(attribute.FileName, attribute.InitialAnimation, provider);
            }
            else
            {
                Animator = new Animator("placeholder", "default", provider);
            }
        }
        public Animator Animator { get; init; }
        public int MirrorFactorHorizontal
        {
            get
            {
                return IsMirroredHorizontal ? -1 : 1;
            }
        }
        public int MirrorFactorVertical
        {
            get
            {
                return IsMirroredVertical ? -1 : 1;
            }
        }
        private SpriteEffects GetFlipping()
        {
            if (IsMirroredVertical && IsMirroredHorizontal)
                return SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;

            if (IsMirroredVertical) return SpriteEffects.FlipVertically;

            if (IsMirroredHorizontal) return SpriteEffects.FlipHorizontally;

            else return SpriteEffects.None;
        }

        protected virtual DrawingParameters DisplayInfo
        {
            get
            {
                var info = new DrawingParameters()
                {
                    Position = this.Position,
                    Mirroring = GetFlipping(),
                };

                foreach (var filter in GetComponents<IDisplayFilter>())
                {
                    info = filter.ApplyFilter(info);
                }

                return info;
            }
        }

        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }

        public IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            if (Invisible)
            {
                yield break;
            }
            yield return Animator.GetVisual(layer.Process(DisplayInfo, camera));
        }

        public virtual void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            Animator.Update(deltaTime);
        }

        public DrawingParameters GetDrawingParameters()
        {
            return DisplayInfo;
        }

        public void Dispose()
        {
            OnDisposeEvent(this);
        }
    }

    public class SpriteSheetAttribute : Attribute
    {
        public string FileName { get; }
        public string InitialAnimation { get; }
        public SpriteSheetAttribute(string fileName)
        {
            FileName = fileName;
            InitialAnimation = "Default";
        }

        public SpriteSheetAttribute(string fileName, string initialAnimation) : this(fileName)
        {
            InitialAnimation = initialAnimation;
        }
    }
}
