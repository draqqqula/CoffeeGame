using MagicDustLibrary.Common;
using MagicDustLibrary.ComponentModel;
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
    /// Может реализовать интерфейсы:
    /// <list type="bullet">
    /// <item><see cref="IDisposable"/> функционал для удаления.</item>
    /// <item><see cref="IDisplayComponent"/> функционал для отрисовки</item>
    /// <item><see cref="IBodyComponent"/> функционал для коллизии</item>
    /// <item><see cref="IUpdateComponent"/> функционал для обновления на каждом кадре</item>
    /// <item><see cref="IFamilyComponent"/> возможность быть членом <see cref="IFamily"/></item>
    /// <item><see cref="IMultiBehaviorComponent"/> возможность добавлять функционал через <see cref="IBehavior"/></item>
    /// </list>
    /// </summary>
    public abstract class GameObject : ComponentShell, IDisposableComponent
    {
        public event OnDispose OnDisposeEvent = delegate { };

        public void Dispose()
        {
            OnDisposeEvent?.Invoke(this);
        }
    }
}
