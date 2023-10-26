using MagicDustLibrary.Animations;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace MagicDustLibrary.CommonObjectTypes
{
    public abstract class Sprite : GameObject
    {
        public Animator Animator { get; init; }
        public override IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer)
        {
            yield return Animator.GetVisual(layer.Process(DisplayInfo, camera));
        }

        public virtual void OnTick(IStateController state, TimeSpan deltaTime)
        {
        }
        public void UpdateAnimator(IStateController state, TimeSpan deltaTime)
        {
            Animator.Update(deltaTime);
        }

        protected Sprite(IPlacement placement, Vector2 position, IAnimationProvider provider) : base(placement, position)
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
            OnUpdate += OnTick;
            OnUpdate += UpdateAnimator;
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
