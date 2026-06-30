using UnityEngine;

namespace RunnerGame.Camera
{
    /// <summary>
    /// Smoothly follows the player's position using a customizable spatial offset 
    /// and interpolation dampening speeds configured via the Unity Inspector.
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Target Link")]
        [Tooltip("Assign the main Player transform execution shell here.")]
        [SerializeField] private Transform playerTarget;

        [Header("Position Customization (Coordinates)")]
        [Tooltip("The manual spatial distance coordinates (X, Y, Z) separating the camera from the player.")]
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 6f, -7f);

        [Header("Movement Smoothness Settings")]
        [Tooltip("The interpolation factor controlling how smoothly the camera catches up to the player.")]
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Lane Clamping Restrictions (Optional)")]
        [Tooltip("If true, the camera will lock onto a fixed X coordinate and only follow forward/upward tracks.")]
        [SerializeField] private bool lockHorizontalX = false;
        [SerializeField] private float fixedHorizontalX = 0f;

        private void Start()
        {
            // Fallback safety filter to auto-locate the player if the field is forgotten in the Inspector
            if (playerTarget == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTarget = playerObj.transform;
                }
            }
        }

        /// <summary>
        /// LateUpdate runs after all standard Update movement cycles finish. 
        /// This completely eliminates micro-stuttering and jittering bugs.
        /// </summary>
        private void LateUpdate()
        {
            if (playerTarget == null) return;

            // Calculate the absolute raw target location by combining target matrices with our manual offset coordinates
            Vector3 desiredPosition = playerTarget.position + cameraOffset;

            // Apply horizontal constraints if the developer wants a strict straight-forward camera view
            if (lockHorizontalX)
            {
                desiredPosition.x = fixedHorizontalX;
            }

            // Smoothly blend the camera's current position toward the desired location using stable frame-rate independent linear filters
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Commit the calculated coordinates directly to the camera transform layout
            transform.position = smoothedPosition;
        }
    }
}