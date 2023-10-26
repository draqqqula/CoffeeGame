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

    public class MemberShipAttribute : Attribute
    {
        public string FamilyName { get; }
        public MemberShipAttribute(string familyName)
        {
            FamilyName = familyName;
        }
    }
    #endregion

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
                return new DrawingParameters()
                {
                    Position = this.Position,
                    Mirroring = GetFlipping(),
                };
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
                behavior.Act(this, deltaTime);
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
