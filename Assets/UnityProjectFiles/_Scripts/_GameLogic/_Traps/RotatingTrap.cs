using UnityEngine;
using RunnerGame.Player;

namespace RunnerGame.Obstacles
{
    /// <summary>
    /// Rotates an obstacle continuously and applies a backward knockback force to the player upon collision.
    /// </summary>
    public class RotatingTrap : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 180f, 0f); // Degrees per second

        [Header("Knockback Settings")]
        [SerializeField] private float knockbackPunchForce = 8f;
        [SerializeField] private float knockbackUpwardForce = 3f;
        [SerializeField] private float knockbackDuration = 0.4f;

        private void Update()
        {
            // Rotate the trap smoothly over time frame rates
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verify if the colliding object is the player controller
            if (other.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                // Calculate backward direction vector based on impact
                Vector3 knockbackDir = (player.transform.position - transform.position).normalized;
                knockbackDir.y = 0f; // Keep horizontal flat planes

                // Combine pushing back direction with an upward lifting arc
                Vector3 finalForce = (knockbackDir * knockbackPunchForce) + (Vector3.up * knockbackUpwardForce);

                // Trigger the core movement controller knockback routine
                player.ApplyKnockback(finalForce, knockbackDuration);
            }
        }
    }
}