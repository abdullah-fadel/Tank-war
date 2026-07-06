using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Enemies.AIStates
{
    /// <summary>Moves toward the player's X position while keeping a minimum stand-off distance.</summary>
    public class ChaseState : IState
    {
        private readonly Transform _transform;
        private readonly EnemyBase _enemy;
        private readonly float _speed;
        private readonly float _stopDistance;

        public ChaseState(Transform transform, EnemyBase enemy, float speed, float stopDistance)
        {
            _transform = transform;
            _enemy = enemy;
            _speed = speed;
            _stopDistance = stopDistance;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            if (_enemy.PlayerTransform == null) return;
            if (_enemy.DistanceToPlayer() <= _stopDistance) return;

            float direction = Mathf.Sign(_enemy.PlayerTransform.position.x - _transform.position.x);
            var pos = _transform.position;
            pos.x += direction * _speed * deltaTime;
            _transform.position = pos;

            var scale = _transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            _transform.localScale = scale;
        }

        public void FixedTick(float fixedDeltaTime) { }
        public void Exit() { }
    }
}
