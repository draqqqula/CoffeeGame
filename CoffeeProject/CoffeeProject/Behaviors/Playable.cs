using BehaviorKit;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Logic.Behaviors;
using MagicDustLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;

namespace CoffeeProject.Behaviors
{
    public class Playable : Behavior<IMultiBehaviorComponent>
    {
        private Dummy Dummy;
        private float HealthDelta;
        private TimerHandler TimerHandler;
        private Label HealthBar;

        private const double DecayTime = 0.1;
        private bool IsOnDecay = false;

        protected override void Act(IControllerProvider state, TimeSpan deltaTime, IMultiBehaviorComponent parent)
        {
            StringBuilder healthText = new StringBuilder();
            for (int i = 1; i <= Dummy.MaxHealth; i++)
                healthText.Append(i <= Dummy.Health ? 'a' : (i <= HealthDelta ? 'c' : 'b'));
            HealthBar.Text = healthText.ToString();

            if (HealthDelta > Dummy.Health)
            {
                if (!IsOnDecay && TimerHandler.When("HealthBarDecay", DecayTime))
                    IsOnDecay = true;
                else if (IsOnDecay)
                    HealthDelta = MathEx.Catch(MathEx.Lerp(HealthDelta, Dummy.Health, 0.2f, deltaTime), Dummy.Health, 0.04f);
            }
            else if (HealthDelta < Dummy.Health)
            {
                HealthDelta = Dummy.Health;
                IsOnDecay = false;
            }
            else
            {
                IsOnDecay = false;
            }
        }

        public void ContactDummy(Dummy dummy)
        {
            Dummy = dummy;
            HealthDelta = dummy.Health;
        }

        public void ContactTimer(TimerHandler timer)
        {
            TimerHandler = timer;
        }

        public Playable(Label healthBar) : base()
        {
            HealthBar = healthBar;
            AddGreetingFor<Dummy>(ContactDummy);
            AddGreetingFor<TimerHandler>(ContactTimer);
        }
    }
}