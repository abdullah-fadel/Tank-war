using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>Trigger volume placed at the end of a Story/Boss level track; completes the level when the player reaches it.</summary>
    [RequireComponent(typeof(Collider))]
    public class LevelEndZone : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            LevelManager.Instance?.CompleteLevel();
        }
    }
}
