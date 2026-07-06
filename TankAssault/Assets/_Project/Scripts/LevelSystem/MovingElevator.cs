using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>Platform that oscillates between two points; carries the player by parenting on contact.</summary>
    public class MovingElevator : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float speed = 2f;
        [SerializeField] private string playerTag = "Player";

        private Vector3 _target;

        private void Start()
        {
            if (pointA != null) transform.position = pointA.position;
            _target = pointB != null ? pointB.position : transform.position;
        }

        private void Update()
        {
            if (pointA == null || pointB == null) return;

            transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _target) < 0.05f)
                _target = _target == pointA.position ? pointB.position : pointA.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(playerTag))
                collision.transform.SetParent(transform);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.CompareTag(playerTag))
                collision.transform.SetParent(null);
        }
    }
}
