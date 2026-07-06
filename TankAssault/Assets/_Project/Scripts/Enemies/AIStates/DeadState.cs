using TankAssault.Core;

namespace TankAssault.Enemies.AIStates
{
    /// <summary>Terminal state; movement/attack stop, EnemyBase.OnDeath already handles VFX/rewards/despawn.</summary>
    public class DeadState : IState
    {
        public void Enter() { }
        public void Tick(float deltaTime) { }
        public void FixedTick(float fixedDeltaTime) { }
        public void Exit() { }
    }
}
