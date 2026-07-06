using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Enemies.AIStates
{
    /// <summary>Retreats away from the player, used by low-health snipers/drones that disengage at close range.</summary>
    public class FleeState : IState
    {
        private readonly Transform _transform;
        private readonly EnemyBase _enemy;
        private readonly float _speed;

        public FleeState(Transform transform, EnemyBase enemy, float speed)
        {
            _transform = transform;
            _enemy = enemy;
            _speed = speed;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            if (_enemy.PlayerTransform == null) return;

            float direction = -Mathf.Sign(_enemy.PlayerTransform.position.x - _transform.position.x);
            var pos = _transform.position;
            pos.x += direction * _speed * deltaTime;
            _transform.position = pos;
        }

        public void FixedTick(float fixedDeltaTime) { }
        public void Exit() { }
    }
}
