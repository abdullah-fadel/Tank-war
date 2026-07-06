using TankAssault.Core;
using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>Trigger volume marking a checkpoint; the level manager respawns the player here on death.</summary>
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private int checkpointIndex;
        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            LevelManager.Instance?.SetActiveCheckpoint(checkpointIndex, transform.position);
            EventBus.Publish(new CheckpointReachedEvent(checkpointIndex));
        }
    }
}
