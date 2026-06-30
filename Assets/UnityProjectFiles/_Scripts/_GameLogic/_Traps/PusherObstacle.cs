using UnityEngine;
using RunnerGame.Player;

namespace RunnerGame.Obstacles
{
    /// <summary>
    /// Rotates an obstacle like a windmill or turnstile, using solid physical collisions (Non-Trigger).
    /// Delivers a gentle, non-lethal nudge to slightly push and stagger the player upon contact.
    /// </summary>
    public class PusherObstacle : MonoBehaviour
    {
        [Header("Windmill Rotation Settings")]
        [SerializeField] private Vector3 rotationDirectionAndSpeed = new Vector3(0f, 90f, 0f); // Rotates on Y-axis by default

        [Header("Gentle Push Settings")]
        [SerializeField] private float sidePushForce = 4f;       // Subtle horizontal push to nudge the player aside
        [SerializeField] private float upwardLiftForce = 2f;     // Very slight lift to make the push visible
        [SerializeField] private float backwardStaggerForce = 6f; // Small deceleration push down the lane
        [SerializeField] private float staggerDuration = 0.25f;  // Quick control recovery window

        private void Update()
        {
            // Continuously rotate the windmill arms in world or local space
            transform.Rotate(rotationDirectionAndSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Verify collision with the player movement architecture using solid physics matrix contacts
            if (collision.gameObject.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                // Calculate the hit normal vector to know which side of the arm clobbered the player
                Vector3 contactNormal = collision.contacts[0].normal;
                
                // Determine lateral direction based on contact points (-1 for Left, 1 for Right)
                float sideSign = player.transform.position.x > transform.position.x ? 1f : -1f;

                // Build a gentle, restrained knockback vector to just bump the player without launching them out of bounds
                Vector3 gentleNudge = new Vector3(
                    sideSign * sidePushForce,     // Soft lateral slide
                    upwardLiftForce,              // Tiny bounce hop
                    -backwardStaggerForce         // Brief pacing deceleration
                );

                Debug.Log($"<color=lightblue>[Windmill Pusher]</color> Gently bumping player in direction: {gentleNudge}");

                // Apply the calculated gentle physics trauma to the player handler
                player.ApplyKnockback(gentleNudge, staggerDuration);
            }
        }
    }
}