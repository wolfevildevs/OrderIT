using UnityEngine;
using RunnerGame.Audio;

namespace RunnerGame.Player
{
    /// <summary>
    /// Handles the physics-based movement, mobile touch inputs, custom gravity,
    /// and dynamic character leaning/rotation for the runner player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerDataSO playerData;

        [Header("Mobile Touch Settings")]
        [SerializeField] private float touchSensitivity = 0.5f; 

        [Header("Fall Bounds Settings")]
        [SerializeField] private float fallThresholdY = -5f; 

        [Header("Ground Check Settings (SphereCast)")]
        [SerializeField] private float groundCheckRadius = 0.3f;      
        [SerializeField] private float groundCheckDistance = 0.4f;    
        [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, 0.2f, 0f); 

        [Header("Juicy Rotation Settings")]
        [SerializeField] private float rightRotationAngle = 25f;  
        [SerializeField] private float leftRotationAngle = -25f; 
        [SerializeField] private float rotationSmoothSpeed = 10f; 

        [Header("Mobile Swipe Settings (Percentage Based)")]
        [Tooltip("Percentage of screen height required to trigger a jump. e.g., 0.08 = 8% of screen height.")]
        [SerializeField] private float swipeUpThresholdPercent = 0.08f;

        private Rigidbody rb;
        private bool isGrounded;
        private Vector2 touchStartPos;
        private Vector2 initialTouchStartPos; 
        private float horizontalInputFromTouch = 0f;
        private bool isKnockedBack = false;
        private float knockbackTimer = 0f;
        private bool hasSwipedUpInCurrentTouch = false;
        private float jumpCooldownTimer = 0f;

        // Exposed property for PlayerVisualsManager animation syncs
        public bool IsGrounded => isGrounded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = false; 
            isGrounded = true; 
        }

        private void Start()
        {
            if (playerData != null)
            {
                playerData.ResetData();
                // FIXED: Stopped the controller from forcing movement on start natively
                playerData.isMoving = false; 
            }
            CheckGrounded();
        }

        private void Update()
        {
            if (playerData == null || playerData.isDead) return;

            // FIXED: Prevent any input processing or movement bounds logic if the game has not started
            if (!playerData.isMoving) return;

            // Update timers
            if (jumpCooldownTimer > 0f)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }

            CheckGrounded();

            if (transform.position.y < fallThresholdY)
            {
                Die();
                return;
            }

            if (isKnockedBack)
            {
                knockbackTimer -= Time.deltaTime;
                if (knockbackTimer <= 0) isKnockedBack = false;
                return; 
            }

            HandleInputs();
        }

        private void FixedUpdate()
        {
            // FIXED: Blocked physical physics updates completely if player movement is locked by architecture
            if (playerData == null || playerData.isDead || isKnockedBack || !playerData.isMoving) return;

            MovePlayer();
        }

        private void MovePlayer()
        {
            float forwardVelocity = playerData.forwardSpeed;
            float keyboardInput = Input.GetAxis("Horizontal");
            float finalHorizontalInput = keyboardInput != 0 ? keyboardInput : horizontalInputFromTouch;

            float targetX = rb.position.x + (finalHorizontalInput * playerData.sideMovementSpeed * Time.fixedDeltaTime);
            targetX = Mathf.Clamp(targetX, -playerData.roadLimitX, playerData.roadLimitX);

            float desiredHorizontalVelocity = (targetX - rb.position.x) / Time.fixedDeltaTime;
            float yVelocity = rb.linearVelocity.y;
            
            if (isGrounded && jumpCooldownTimer <= 0f)
            {
                if (yVelocity > 0) yVelocity = 0f;
            }
            else
            {
                yVelocity += playerData.customGravity * Time.fixedDeltaTime;
            }

            rb.linearVelocity = new Vector3(desiredHorizontalVelocity, yVelocity, forwardVelocity);
            ApplyJuicyRotation(finalHorizontalInput);

            horizontalInputFromTouch = Mathf.Lerp(horizontalInputFromTouch, 0f, Time.fixedDeltaTime * 10f);
        }

        private void ApplyJuicyRotation(float horizontalInput)
        {
            float targetYAngle = 0f;

            if (horizontalInput > 0.05f) targetYAngle = rightRotationAngle;
            else if (horizontalInput < -0.05f) targetYAngle = leftRotationAngle;

            Quaternion targetRotation = Quaternion.Euler(0f, targetYAngle, 0f);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSmoothSpeed);
        }

        private void HandleInputs()
        {
            if (Input.GetButtonDown("Jump") && isGrounded && jumpCooldownTimer <= 0f)
            {
                Jump();
            }

            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                initialTouchStartPos = Input.mousePosition; 
                hasSwipedUpInCurrentTouch = false; 
            }
            else if (Input.GetMouseButton(0)) 
            {
                Vector2 currentTouchPos = Input.mousePosition;
                Vector2 cleanSwipeDelta = currentTouchPos - initialTouchStartPos;

                float currentSwipeHeightRatio = cleanSwipeDelta.y / Screen.height;

                if (!hasSwipedUpInCurrentTouch && currentSwipeHeightRatio > swipeUpThresholdPercent && isGrounded && jumpCooldownTimer <= 0f)
                {
                    Debug.Log("<color=green>[Movement] Swipe Up Detected! Jumping now.</color>");
                    Jump();
                    hasSwipedUpInCurrentTouch = true; 
                }

                float touchDeltaX = currentTouchPos.x - touchStartPos.x;
                horizontalInputFromTouch = (touchDeltaX / Screen.width) * touchSensitivity * 100f;
                
                touchStartPos = Vector2.Lerp(touchStartPos, currentTouchPos, Time.deltaTime * 5f);
            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                horizontalInputFromTouch = 0f;
                hasSwipedUpInCurrentTouch = false;
            }
        }

        private void Jump()
        {
            jumpCooldownTimer = 0.15f;
            isGrounded = false; 
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, playerData.jumpForce, rb.linearVelocity.z);
        }

        private void CheckGrounded()
        {
            if (jumpCooldownTimer > 0f)
            {
                isGrounded = false;
                return;
            }

            Vector3 origin = transform.position + groundCheckOffset;
            isGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance);
        }

        public void ApplyKnockback(Vector3 force, float duration)
        {
            if (playerData.isDead) return;
            
            isKnockedBack = true;
            knockbackTimer = duration;
            
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(force, ForceMode.Impulse);
        }

        public void Die()
        {
            if (playerData != null) playerData.isDead = true;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            AudioManager.Instance.PlaySFX("death");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 startCenter = transform.position + groundCheckOffset;
            Vector3 endCenter = startCenter + (Vector3.down * groundCheckDistance);
            
            Gizmos.DrawWireSphere(startCenter, groundCheckRadius);
            Gizmos.DrawWireSphere(endCenter, groundCheckRadius);
        }
    }
}