using BehaviorKit;
using CoffeeProject.Layers;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.ComponentModel;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.GameObjects
{
    internal class HealthLabel : Label, IUpdateComponent
    {
        private Dummy Dummy { get; set; }
        private IBodyComponent Body { get; set; }
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public HealthLabel()
        {
            AddGreetingFor<Dummy>(it =>
            {
                Dummy = it;
                Dummy.OnDisposeEvent += (it) => this.Dispose();
            });
            AddGreetingFor<IBodyComponent>(it =>
            {
                Body = it;
                Body.OnDisposeEvent += (it) => this.Dispose();
            });
        }

        public void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            if (Body is null || Dummy is null)
            {
                return;
            }

            Position = Body.Position + Offset + new Vector2(-Bounds.Width/2, 0);
            this.SetText($"{Dummy.Health}/{Dummy.MaxHealth}");
        }
    }

    public static class HealthLabelExtensions
    {
        public static T AddHealthLabel<T>(this T obj, IControllerProvider state) where T : GameObject
        {
            obj.CombineWith(new HealthLabel().SetScale(0.4f).UseFont(state, "Caveat").SetPlacement(new Placement<MainLayer>()));
            return obj;
        }
    }
}
