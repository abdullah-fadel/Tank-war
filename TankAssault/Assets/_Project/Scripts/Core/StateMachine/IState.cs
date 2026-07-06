namespace TankAssault.Core
{
    /// <summary>Generic FSM state contract. Implemented by both player-side and AI-side states.</summary>
    public interface IState
    {
        void Enter();
        void Tick(float deltaTime);
        void FixedTick(float fixedDeltaTime);
        void Exit();
    }
}
