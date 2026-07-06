using TankAssault.Core;

namespace TankAssault.Enemies.AIStates
{
    /// <summary>Enemy stands still, typically until the player enters detection range.</summary>
    public class IdleState : IState
    {
        public void Enter() { }
        public void Tick(float deltaTime) { }
        public void FixedTick(float fixedDeltaTime) { }
        public void Exit() { }
    }
}
