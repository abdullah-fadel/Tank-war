using System;
using TankAssault.Core;

namespace TankAssault.Enemies.AIStates
{
    /// <summary>Generic cooldown-gated attack; the actual attack logic is supplied per enemy type via onAttack.</summary>
    public class AttackState : IState
    {
        private readonly Action _onAttack;
        private readonly Func<float> _getCooldown;
        private float _timer;

        public AttackState(Action onAttack, Func<float> getCooldown)
        {
            _onAttack = onAttack;
            _getCooldown = getCooldown;
        }

        public void Enter()
        {
            _timer = 0f;
        }

        public void Tick(float deltaTime)
        {
            _timer -= deltaTime;
            if (_timer <= 0f)
            {
                _onAttack?.Invoke();
                _timer = _getCooldown();
            }
        }

        public void FixedTick(float fixedDeltaTime) { }
        public void Exit() { }
    }
}
