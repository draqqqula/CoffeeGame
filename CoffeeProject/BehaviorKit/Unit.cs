using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorKit
{
    public class Unit<TUnit, TTarget> : Behavior<TUnit> where TUnit : class, IMultiBehaviorComponent where TTarget : GameObject
    {
        private TimerHandler timerHandler;

        private Dictionary<string, int> StepCooldowns = [];

        private readonly Dictionary<string, UnitMove<TUnit, TTarget>> Actions = [];

        private KeyValuePair<string, UnitMove<TUnit, TTarget>> CurrentAction;

        public TTarget Target { get; set; }

        #region TARGET
        public void SetTarget(IControllerProvider state, TTarget target, TUnit parent)
        {
            if (target != Target)
            {
                LoseTarget();
                Target = target;
                TakeAction(state, SearchForAction(target, parent), parent);
            }
        }

        public void LoseTarget()
        {
            timerHandler.CheckAndTurnOff("Action");
            timerHandler.CheckAndTurnOff("Cooldown");
            Target = null;
        }
        #endregion

        #region ACTIONS

        public bool HasTarget
        {
            get => Target is not null;
        }

        private void ReactWith(IControllerProvider state, KeyValuePair<string, UnitMove<TUnit, TTarget>> action, TUnit parent)
        {
            if (CurrentAction.Value is not null)
                CurrentAction.Value.OnForcedBreak(state, parent, Target, action.Value);
            timerHandler.Silence("Action");
            timerHandler.Silence("Cooldown");
            TakeAction(state, action, parent);
        }

        public void ReactWith(IControllerProvider state, string actionName, TUnit parent)
        {
            ReactWith(state, new KeyValuePair<string, UnitMove<TUnit, TTarget>>(actionName, Actions[actionName]), parent);
        }
        public void ReactWith<Move>(IControllerProvider state, TUnit parent) where Move : UnitMove<TUnit, TTarget>
        {
            ReactWith(state, typeof(Move).Name, parent);
        }

        public void TakeAction(IControllerProvider state, KeyValuePair<string, UnitMove<TUnit, TTarget>> action, TUnit parent)
        {
            Step();
            CurrentAction = action;

            if (action.Value.HasCooldown)
                timerHandler.SetTimer(action.Key, action.Value.Cooldown, false);

            if (action.Value.IsTurnDependent)
                StepCooldowns[action.Key] = action.Value.StepsToTurn;

            if (!action.Value.IsEndless)
            {
                timerHandler.SetTimer("Action", action.Value.Duration, () => action.Value.OnEnd(state, parent, Target), false);
                if (action.Value.HasFreeSpan)
                    timerHandler.SetTimer("Cooldown", action.Value.FreeSpan + action.Value.Duration, () => TakeAction(state, SearchForAction(Target, parent), parent), false);
            }

            action.Value.OnStart(state, parent, Target);
        }

        public KeyValuePair<string, UnitMove<TUnit, TTarget>> SearchForAction(TTarget target, TUnit parent)
        {
            return Actions
                .Where(action => !ActionOnCooldown(action.Key))
                .DefaultIfEmpty()
                .OrderByDescending(action => action.Value.GetAttraction(parent, target))
                .ThenBy(action => action.Value.Priority)
                .First();
        }

        public bool ActionOnCooldown(string name)
        {
            return timerHandler.CheckAndTurnOff(name) == TimerState.Running || StepCooldowns.ContainsKey(name);
        }

        public void Step()
        {
            foreach (var cooldown in StepCooldowns.Keys)
                StepCooldowns[cooldown] -= 1;
            StepCooldowns = StepCooldowns.Where(cooldown => cooldown.Value > 0).ToDictionary(e => e.Key, e => e.Value);
        }

        public string ActionName
        {
            get => CurrentAction.Key;
        }

        public double Progress
        {
            get
            {
                double progress;
                if (timerHandler.TryGetProgress("Action", out progress))
                    return progress;
                else
                    return 0;
            }
        }

        public void AddAction(string name, UnitMove<TUnit, TTarget> action)
        {
            Actions.Add(name, action);
        }

        public void AddActions(params (string name, UnitMove<TUnit, TTarget> action)[] actions)
        {
            foreach (var action in actions)
            {
                AddAction(action.name, action.action);
            }
        }
        #endregion

        #region IBEHAVIOR
        protected override void Act(IControllerProvider state, TimeSpan deltaTime, TUnit parent)
        {
            if (CurrentAction.Value != null && !CurrentAction.Value.Continue(state, parent, Target))
            {
                var action = CurrentAction.Value;
                timerHandler.CheckAndTurnOff("Action");

                action.OnEnd(state, parent, Target);

                if (action.HasFreeSpan)
                {
                    timerHandler.Hold("Cooldown", action.FreeSpan, () => TakeAction(state, SearchForAction(Target, parent), parent), false);
                }
                else
                {
                    TakeAction(state, SearchForAction(Target, parent), parent);
                }

                CurrentAction = default;
            }
        }
        #endregion

        public Unit()
        {
            AddGreetingFor<TimerHandler>(it => timerHandler = it);
        }
    }

    public abstract class UnitMove<TUnit, TTarget> where TUnit : class, IMultiBehaviorComponent where TTarget : GameObject
    {
        #region STATUS
        public bool Enabled { get; private set; }
        public void Enable() => Enabled = true;
        public void Disable() => Enabled = false;
        #endregion

        #region PROPERTIES
        public readonly int Priority;
        public readonly TimeSpan Duration;
        public readonly TimeSpan Cooldown;
        public readonly int StepsToTurn;
        public readonly TimeSpan FreeSpan;
        #endregion

        #region RULES
        public readonly bool IsEndless;
        public readonly bool HasCooldown;
        public readonly bool IsTurnDependent;
        public readonly bool HasFreeSpan;

        #endregion

        #region SCRIPT
        public virtual void OnStart(IControllerProvider state, TUnit unit, TTarget target) { }
        public virtual void OnEnd(IControllerProvider state, TUnit unit, TTarget target) { }
        public virtual bool Continue(IControllerProvider state, TUnit unit, TTarget target) { return true; }
        public virtual void OnForcedBreak(IControllerProvider state, TUnit unit, TTarget target, UnitMove<TUnit, TTarget> breaker) { }
        #endregion

        #region CHOICE
        public abstract int GetAttraction(TUnit unit, TTarget target);
        #endregion

        #region CREATION
        public UnitMove()
        {
            var attributes = GetType().GetCustomAttributes(true);
            Priority = 1;
            foreach (var attribute in attributes)
            {
                if (attribute is MoveDurationAttribute moveDuration)
                {
                    Duration = moveDuration.Duration;
                    IsEndless = false;
                }
                else if (attribute is EndlessMoveAttribute)
                {
                    Duration = TimeSpan.Zero;
                    IsEndless = true;
                }
                else if (attribute is MoveFreeSpanAttribute moveFreeSpan)
                {
                    FreeSpan = moveFreeSpan.Duration;
                    HasFreeSpan = true;
                }
                else if (attribute is NoFreeSpanMoveAttribute)
                {
                    FreeSpan = TimeSpan.Zero;
                    HasFreeSpan = false;
                }
                else if (attribute is MoveStepsRequiredAttribute moveStepsRequired)
                {
                    StepsToTurn = moveStepsRequired.Count;
                    IsTurnDependent = true;
                }
                else if (attribute is TurnIndependentMoveAttribute)
                {
                    StepsToTurn = 0;
                    IsTurnDependent = false;
                }
                else if (attribute is MoveCooldownAttribute moveCooldown)
                {
                    Cooldown = moveCooldown.Duration;
                    HasCooldown = true;
                }
                else if (attribute is NoCooldownMoveAttribute)
                {
                    Cooldown = TimeSpan.Zero;
                    HasCooldown = false;
                }
                else if (attribute is MovePriorityAttribute priority)
                {
                    Priority = priority.Number;
                }
            }
        }
        #endregion
    }
    #region UNITMOVE ATTRIBUTES
    public class MoveDurationAttribute : Attribute
    {
        public TimeSpan Duration { get; }
        public MoveDurationAttribute(double seconds)
        {
            Duration = TimeSpan.FromSeconds(seconds);
        }
    }
    public class MoveFreeSpanAttribute : Attribute
    {
        public TimeSpan Duration { get; }
        public MoveFreeSpanAttribute(double seconds)
        {
            Duration = TimeSpan.FromSeconds(seconds);
        }
    }

    public class MoveStepsRequiredAttribute : Attribute
    {
        public int Count { get; }
        public MoveStepsRequiredAttribute(int count)
        {
            Count = count;
        }
    }

    public class MovePriorityAttribute : Attribute
    {
        public int Number { get; }
        public MovePriorityAttribute(int number)
        {
            Number = number;
        }
    }

    public class MoveCooldownAttribute : Attribute
    {
        public TimeSpan Duration { get; }
        public MoveCooldownAttribute(double seconds)
        {
            Duration = TimeSpan.FromSeconds(seconds);
        }
    }

    public class EndlessMoveAttribute : Attribute { }
    public class TurnIndependentMoveAttribute : Attribute { }
    public class NoFreeSpanMoveAttribute : Attribute { }
    public class NoCooldownMoveAttribute : Attribute { }
    #endregion
}
