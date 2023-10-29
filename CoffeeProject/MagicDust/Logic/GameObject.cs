using MagicDustLibrary.Display;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace MagicDustLibrary.Logic
{

    #region GAMEOBJECT ATTRIBUTES
    public class CloneableAttribute : Attribute
    {
    }

    public class CustomCloningAttribute : Attribute
    {
    }

    public class ReversedVisibilityAttribute : Attribute
    {
    }

    public class NoVisualsAttribute : Attribute
    {
    }

    public class BoxAttribute : Attribute
    {
        public Rectangle Rectangle { get; }

        public BoxAttribute(int halfSize)
        {
            Rectangle = new Rectangle(-halfSize, -halfSize, halfSize * 2, halfSize * 2);
        }

        public BoxAttribute(int width, int height, int pivotX, int pivotY)
        {
            Rectangle = new Rectangle(-pivotX, -pivotY, width, height);
        }

        public BoxAttribute(int halfWidth, int halfHeight)
        {
            Rectangle = new Rectangle(-halfWidth, -halfHeight, halfWidth * 2, halfHeight * 2);
        }
    }

    public interface IMemberShipContainer
    {
        public Type FamilyType { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MemberShipAttribute<F> : Attribute, IMemberShipContainer where F : IFamily
    {
        public Type FamilyType { get; }
        public MemberShipAttribute()
        {
            FamilyType = typeof(F);
        }
    }
    #endregion

    /// <summary>
    /// Главный родительский класс для объектов.<br/><br/>
    /// Реализует интерфейсы:
    /// <list type="bullet">
    /// <item><see cref="IDisposable"/> функционал для удаления.</item>
    /// <item><see cref="IDisplayProvider"/> функционал для отрисовки</item>
    /// <item><see cref="IBody"/> функционал для коллизии</item>
    /// <item><see cref="IStateUpdateable"/> функционал для обновления на каждом кадре</item>
    /// <item><see cref="IFamilyMember"/> возможность быть членом <see cref="IFamily"/></item>
    /// <item><see cref="IMultiBehavior"/> возможность добавлять функционал через <see cref="IBehavior"/></item>
    /// </list>
    /// </summary>
    public abstract class GameObject : IDisposable, IDisplayProvider, IBody, IStateUpdateable, IFamilyMember, IMultiBehavior
    {

        #region IDisposable
        public Action<GameObject>? OnDisposed;

        public void Dispose()
        {
            if (OnDisposed is not null)
            {
                OnDisposed(this);
            }
        }

        #endregion


        #region IDisplayProvider
        public bool IsMirroredVertical = false;
        public bool IsMirroredHorizontal = false;
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
                foreach (var behavior in Behaviors.Values)
                {
                    info = behavior.ChangeAppearanceUnhandled(this, info);
                }
                return info;
            }
        }

        public abstract IEnumerable<IDisplayable> GetDisplay(GameCamera camera, Layer layer);

        public bool IsVisibleFor(GameClient client)
        {
            return ReversedVisibility ^ !ClientList.Contains(client);
        }

        public Type GetLayerType()
        {
            return Placement.GetLayerType();
        }

        public IPlacement Placement { get; private set; }
        private readonly HashSet<GameClient> ClientList = new();
        private readonly bool ReversedVisibility;
        #endregion


        #region IStateUpdateAble
        public virtual void Update(IStateController state, TimeSpan deltaTime)
        {
            foreach (var behavior in Behaviors.Values)
            {
                behavior.UpdateUnhandled(state, deltaTime, this);
            }
            OnUpdate(state, deltaTime);
        }

        public Action<IStateController, TimeSpan> OnUpdate = (state, deltaTime) => { };
        #endregion


        #region IBody
        public virtual Vector2 Position { get; protected set; }
        public virtual Rectangle Box { get; protected set; }
        public Vector2 GetPosition()
        {
            return Position;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetBounds(Rectangle box)
        {
            Box = box;
        }

        public Rectangle GetBounds()
        {
            return Box;
        }

        public Rectangle Layout
        {
            get => new Rectangle(Box.Location + Position.ToPoint(), Box.Size);
        }

        public Rectangle PredictLayout(Vector2 movementPrediction)
        {
            return new
                Rectangle(
                (int)(Position.X + movementPrediction.X) + Box.X,
                (int)(Position.Y + movementPrediction.Y) + Box.Y,
                Box.Width,
                Box.Height
                );
        }
        #endregion


        #region NETWORK

        public byte[] LinkedID { get; private set; }

        public void Link(byte[] bytes)
        {
            if (bytes.Length != 16) throw new ArgumentException("Invalid length of LinkID");
            LinkedID = bytes;
        }

        #endregion


        #region CONSTRUCTORS
        protected Action<GameObject> OnAssembled = (obj) => { };

        public GameObject(IPlacement placement, Vector2 position)
        {
            Placement = placement;
            Position = position;

            bool reversedVisibility = false;
            Rectangle hitbox = new Rectangle(-64, -64, 128, 128);

            ParseAttributes(this.GetType().GetCustomAttributes(true), ref hitbox, ref reversedVisibility);

            Box = hitbox;
            ReversedVisibility = reversedVisibility;
        }
        private void ParseAttributes(object[] attributes, ref Rectangle hitbox, ref bool reversedVisibility)
        {
            foreach (var attribute in attributes)
            {
                if (attribute is BoxAttribute hb)
                {
                    hitbox = hb.Rectangle;
                }
                else if (attribute is ReversedVisibilityAttribute)
                {
                    reversedVisibility = true;
                }
            }
        }
        #endregion


        #region BEHAVIORS
        public Dictionary<string, IBehavior> Behaviors { get; private set; } = new Dictionary<string, IBehavior>();
        public void AddBehavior(string name, IBehavior behavior)
        {
            Behaviors[name] = behavior;
        }

        public T GetBehavior<T>(string name) where T : IBehavior
        {
            return (T)Behaviors[name];
        }
        #endregion
    }
}
