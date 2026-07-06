using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Enemies.AIStates
{
    /// <summary>Walks back and forth between two X offsets from the spawn point.</summary>
    public class PatrolState : IState
    {
        private readonly Transform _transform;
        private readonly float _speed;
        private readonly float _patrolHalfRange;
        private readonly float _originX;
        private int _direction = 1;

        public PatrolState(Transform transform, float speed, float patrolHalfRange)
        {
            _transform = transform;
            _speed = speed;
            _patrolHalfRange = patrolHalfRange;
            _originX = transform.position.x;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            var pos = _transform.position;
            pos.x += _direction * _speed * deltaTime;

            if (pos.x > _originX + _patrolHalfRange) _direction = -1;
            else if (pos.x < _originX - _patrolHalfRange) _direction = 1;

            _transform.position = pos;

            var scale = _transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(_direction);
            _transform.localScale = scale;
        }

        public void FixedTick(float fixedDeltaTime) { }
        public void Exit() { }
    }
}
