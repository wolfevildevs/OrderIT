using UnityEngine;
using RunnerGame.Player;
using RunnerGame.Audio;

namespace RunnerGame.Obstacles
{
    /// <summary>
    /// A stationary hammer obstacle that splits impact logic into three precise zones based on a customizable offset.
    /// Delivers backward knockback at the center and correct outward lateral launches on the sides.
    /// </summary>
    public class MovingHammerTrap : MonoBehaviour
    {
        [Header("Impact Force Settings")]
        [SerializeField] private float horizontalLaunchForce = 40f; // Massive force to throw player sideways out of bounds
        [SerializeField] private float upwardLiftForce = 8f;        // Vertical lift to bypass ground friction instantly
        [SerializeField] private float backwardPushForce = 25f;     // Severe backward force when hitting the center head

        [Header("Stagger Settings")]
        [SerializeField] private float staggerDuration = 0.5f;      // Player input lock duration

        [Header("Center Zone Customization (Offset & Size)")]
        [Tooltip("The local width threshold from the offset center. Determines the front center penalty zone size.")]
        [SerializeField] private float centerZoneWidth = 0.5f; 
        
        [Tooltip("Manually shift the center penalty zone box away from the model's pivot point to match the visual hammer head.")]
        [SerializeField] private Vector3 centerZoneOffset = Vector3.zero;

        // NOTE: FixedUpdate and Update movement behaviors using Sine waves have been completely stripped out
        // to make the obstacle rely purely on static placement and collision impact matrices.

        private void OnTriggerEnter(Collider other)
        {
            ProcessImpact(other.gameObject);
            AudioManager.Instance.PlaySFX("hammer");
        }

        private void OnCollisionEnter(Collision collision)
        {
            ProcessImpact(collision.gameObject);
            AudioManager.Instance.PlaySFX("hammer");
        }

        /// <summary>
        /// Evaluates where the player hit the hammer relative to the customized center offset box and calculates corrected forces.
        /// </summary>
        private void ProcessImpact(GameObject hitObject)
        {
            if (hitObject.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                // Calculate the player's position relative to the hammer's local space origin
                Vector3 localHitPoint = transform.InverseTransformPoint(player.transform.position);

                // Adjust the local hit calculation by subtracting our custom offset
                float adjustedLocalX = localHitPoint.x - centerZoneOffset.x;

                Vector3 explosiveForce = Vector3.zero;

                // --- 1. CENTER ZONE DETECTED (Hit within the custom offset boundary) ---
                if (adjustedLocalX >= -centerZoneWidth && adjustedLocalX <= centerZoneWidth)
                {
                    // Push straight backwards down the track, adding a slight upward lift
                    explosiveForce = new Vector3(0f, upwardLiftForce, -backwardPushForce);
                    Debug.Log("<color=yellow>[Hammer Impact] CENTER HEAD HIT!</color> Pushing player straight backward.");
                }
                // --- 2. SIDE ZONES DETECTED (Slammed outside the offset box) ---
                else
                {
                    // FIXED DIRECTION LOGIC:
                    // If adjustedLocalX > 0, player is on the right side, so multiply by 1f to launch RIGHT.
                    // If adjustedLocalX < 0, player is on the left side, so multiply by -1f to launch LEFT.
                    float sideSign = adjustedLocalX > 0 ? 1f : -1f;
                    
                    explosiveForce = new Vector3(
                        sideSign * horizontalLaunchForce, // Corrected explosive lateral ejection
                        upwardLiftForce,                  // Disengages ground snapping
                        -10f                              // Minor backward penalty deceleration
                    );
                    Debug.Log($"<color=cyan>[Hammer Impact] SIDE SLAM!</color> Launching player OUTWARD in direction sign: {sideSign}");
                }

                // Apply the final corrected physics vector to the player controller
                player.ApplyKnockback(explosiveForce, staggerDuration);
            }
        }

        /// <summary>
        /// Draws dynamic graphical debug boundaries based on the custom center offset.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Set gizmo reference colors
            Gizmos.color = Color.yellow;

            // Calculate local boundaries relative to the custom centerZoneOffset
            Vector3 leftBoundaryLocal = centerZoneOffset + new Vector3(-centerZoneWidth, 0f, 0f);
            Vector3 rightBoundaryLocal = centerZoneOffset + new Vector3(centerZoneWidth, 0f, 0f);

            // Transform local positions into active global world coordinates
            Vector3 leftBoundaryWorld = transform.TransformPoint(leftBoundaryLocal);
            Vector3 rightBoundaryWorld = transform.TransformPoint(rightBoundaryLocal);

            // Draw vertical boundary lines indicating the penalty zone edges
            Vector3 lineLength = transform.up * 2f; 
            Gizmos.DrawLine(leftBoundaryWorld - lineLength / 2f, leftBoundaryWorld + lineLength / 2f);
            Gizmos.DrawLine(rightBoundaryWorld - lineLength / 2f, rightBoundaryWorld + lineLength / 2f);

            // Set matrix and draw the transparent cube directly aligned with the custom offset
            Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.15f); // Transparent yellow
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(centerZoneOffset, new Vector3(centerZoneWidth * 2f, 2f, 1f));
        }
    }
}