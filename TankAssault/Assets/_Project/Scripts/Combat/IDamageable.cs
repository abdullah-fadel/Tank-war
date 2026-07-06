using UnityEngine;

namespace TankAssault.Combat
{
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly Vector3 HitPoint;
        public readonly Vector3 HitDirection;
        public readonly bool IsCritical;
        public readonly bool IsSplash;
        public readonly GameObject Source;

        public DamageInfo(float amount, Vector3 hitPoint, Vector3 hitDirection, bool isCritical, bool isSplash, GameObject source)
        {
            Amount = amount;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
            IsCritical = isCritical;
            IsSplash = isSplash;
            Source = source;
        }
    }

    /// <summary>Implemented by anything that can take weapon damage: player tank, enemies, breakable walls, barrels.</summary>
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(DamageInfo damage);
    }
}
