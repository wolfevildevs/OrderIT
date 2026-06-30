using UnityEngine;
using RunnerGame.Player;

public class PlayerVisualsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMovementController movementController;

    [Header("Animation Optimization Hashes")]
    private int isMovingHash;
    private int isDeadHash;
    private int isGroundedHash;

    private bool isDeathTriggered = false;

    private void Awake()
    {
        isMovingHash = Animator.StringToHash("isMoving");
        isDeadHash = Animator.StringToHash("isDead");
        isGroundedHash = Animator.StringToHash("isGrounded");
    }

    private void Start()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }
        
        if (movementController == null)
        {
            movementController = GetComponentInParent<PlayerMovementController>();
        }
    }

    private void Update()
    {
        if (playerData == null || playerAnimator == null || movementController == null) return;

        if (!playerData.isDead)
        {
            HandleCoreMovementAnimations();
        }
        else if (!isDeathTriggered)
        {
            HandleDeathAnimation();
        }
    }

    private void HandleCoreMovementAnimations()
    {
        // Sync run state
        playerAnimator.SetBool(isMovingHash, playerData.isMoving);

        // Sync jump state directly from the grounded check logic
        playerAnimator.SetBool(isGroundedHash, movementController.IsGrounded);
    }

    private void HandleDeathAnimation()
    {
        playerAnimator.SetTrigger(isDeadHash);
        isDeathTriggered = true;
        enabled = false; 
    }
}