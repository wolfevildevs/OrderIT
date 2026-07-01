using UnityEngine;
using RunnerGame.Player;

namespace RunnerGame.Obstacles
{
    /// <summary>
    /// Delivers a dramatic physical impulse to the player, pushing them out of bounds
    /// where the player's own boundary checks will handle the official death state.
    /// </summary>
    public class PusherObstacle : MonoBehaviour
    {
        [Header("Push Settings")]
        [SerializeField] private float pushForce = 18f;
        [SerializeField] private float upwardLift = 4f; 
        [SerializeField] private float knockbackDuration = 2f; 

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
                
                if (playerRb != null)
                {
                    // Calculate push direction based on the contact normal matrix
                    Vector3 pushDirection = -collision.contacts[0].normal;
                    
                    pushDirection.y = 0; 
                    pushDirection.z = -0.2f; // Slight backward push to halt forward movement
                    pushDirection.Normalize();

                    // Apply upward lift factor
                    pushDirection += Vector3.up * (upwardLift / pushForce);

                    Vector3 finalForce = pushDirection * pushForce;

                    // Trigger physics and freeze player forward tracking temporarily
                    playerRb.isKinematic = false;
                    player.ApplyKnockback(finalForce, knockbackDuration);

                    Debug.Log($"[Pusher] Player hit! Applied dynamic knockback force: {finalForce}");
                }
            }
        }
    }
}