using TankAssault.Combat;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>Destructible obstacle that blocks movement/line-of-fire until destroyed, revealing hidden paths or shortcuts.</summary>
    public class BreakableWall : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 30f;
        [SerializeField] private GameObject debrisVfxPrefab;
        [SerializeField] private AudioClip breakSfx;
        [SerializeField] private GameObject[] collapseIntoPieces;

        private float _currentHealth;
        public bool IsAlive => _currentHealth > 0f;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;
            _currentHealth -= damage.Amount;
            if (_currentHealth <= 0f) Break();
        }

        private void Break()
        {
            if (debrisVfxPrefab != null)
                PoolManager.Instance.Spawn(debrisVfxPrefab, transform.position, Quaternion.identity);

            if (breakSfx != null)
                AudioSource.PlayClipAtPoint(breakSfx, transform.position);

            foreach (var piece in collapseIntoPieces)
            {
                if (piece == null) continue;
                piece.SetActive(true);
                var rb = piece.GetComponent<Rigidbody>();
                if (rb != null) rb.AddExplosionForce(300f, transform.position, 3f);
            }

            gameObject.SetActive(false);
        }
    }
}
