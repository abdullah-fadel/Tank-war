using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>Reveals a secret area/bonus (extra coins, shortcut) when the player enters, typically behind a BreakableWall.</summary>
    [RequireComponent(typeof(Collider))]
    public class HiddenPathTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject[] revealObjects;
        [SerializeField] private int bonusCoins = 25;
        [SerializeField] private string playerTag = "Player";
        private bool _triggered;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered || !other.CompareTag(playerTag)) return;
            _triggered = true;

            foreach (var obj in revealObjects)
                if (obj != null) obj.SetActive(true);

            if (bonusCoins > 0 && TankAssault.Economy.CurrencyManager.Instance != null)
                TankAssault.Economy.CurrencyManager.Instance.AddCoins(bonusCoins);
        }
    }
}
