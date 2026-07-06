using System;
using System.Collections.Generic;

namespace TankAssault.Core
{
    /// <summary>
    /// Minimal finite state machine used by AI controllers and player systems.
    /// Transitions are evaluated every Tick in priority order, first match wins.
    /// </summary>
    public class StateMachine
    {
        private class Transition
        {
            public readonly IState To;
            public readonly Func<bool> Condition;
            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }

        private readonly Dictionary<IState, List<Transition>> _transitions = new Dictionary<IState, List<Transition>>();
        private readonly List<Transition> _anyTransitions = new List<Transition>();
        private static readonly List<Transition> EmptyList = new List<Transition>();

        public IState CurrentState { get; private set; }

        public void SetState(IState state)
        {
            if (state == CurrentState) return;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (!_transitions.TryGetValue(from, out var list))
            {
                list = new List<Transition>();
                _transitions[from] = list;
            }
            list.Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            _anyTransitions.Add(new Transition(to, condition));
        }

        public void Tick(float deltaTime)
        {
            var transition = GetTransition();
            if (transition != null)
                SetState(transition.To);

            CurrentState?.Tick(deltaTime);
        }

        public void FixedTick(float fixedDeltaTime)
        {
            CurrentState?.FixedTick(fixedDeltaTime);
        }

        private Transition GetTransition()
        {
            foreach (var t in _anyTransitions)
                if (t.Condition()) return t;

            if (CurrentState != null && _transitions.TryGetValue(CurrentState, out var list))
                foreach (var t in list)
                    if (t.Condition()) return t;

            return null;
        }
    }
}
