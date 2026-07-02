using System.Collections.Generic;
using UnityEngine;

namespace RunnerGame.Player
{
    public class PlayerStackManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Transform stackParent; // This is now a separate object in the Scene hierarchy

        [Header("Anchor Follow Settings")]
        [SerializeField] private float anchorFollowSpeed = 15f; // Speed of the external anchor catching up with player
        [SerializeField] private float distanceBehindPlayer = 1f; // Safe space between player and the first box

        [Header("Snake Movement Settings")]
        [SerializeField] private float horizontalFollowSpeed = 9f; 
        [SerializeField] private float forwardFollowSpeed = 30f;    
        [SerializeField] private float boxSpacingZ = 1.3f; 

        [Header("Slope Snapping & Offset Settings")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float raycastDistance = 5f;
        [SerializeField] private float boxHeightOffset = 0.5f; 

        private List<Transform> collectedBoxes = new List<Transform>();
        private bool isStackCleared = false;

        private void Start()
        {
            if (playerData != null)
            {
                playerData.currentBoxCount = 0;
            }
        }

        private void Update()
        {
            if (playerData != null && playerData.isDead) 
            {
                if (!isStackCleared)
                {
                    ClearAndDestroyStack();
                }
                return;
            }

            // --- 1. FOLLOW LOGIC FOR THE EXTERNAL ANCHOR ---
            FollowPlayerWithOffset();

            // --- 2. SNAKE TRAILING FOR INNER BOXES ---
            MoveStackedBoxesElastic();
        }

        private void FollowPlayerWithOffset()
        {
            if (stackParent == null) return;

            // Target position is always behind the player's back by the safe distance amount
            Vector3 targetAnchorPos = transform.position + (Vector3.back * distanceBehindPlayer);
            
            // Keep the anchor exactly locked to the player's horizontal and forward flow smoothly
            stackParent.position = Vector3.Lerp(stackParent.position, targetAnchorPos, Time.deltaTime * anchorFollowSpeed);
            stackParent.rotation = Quaternion.identity;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Simple tag or component verification for collectible cardboard boxes
            if (other.CompareTag("Collectible") || other.gameObject.name.Contains("Box"))
            {
                AddNewBoxToStack(other.transform);
            }
        }

        private void AddNewBoxToStack(Transform boxTransform)
        {
            // Cleanly clear any collection script state first if exists
            if (boxTransform.TryGetComponent<CollectibleBox>(out CollectibleBox cb))
            {
                cb.Collect();
            }

            // Destroying the component instantly wipes it out of the PhysX matrix calculation loop
            if (boxTransform.TryGetComponent<Collider>(out Collider boxCollider))
            {
                Destroy(boxCollider);
            }

            // Completely detach the box from world spaces
            boxTransform.SetParent(null);
            boxTransform.gameObject.layer = LayerMask.NameToLayer("CollectedBoxes");

            // Calculate point origins relative to the external independent anchor
            Vector3 spawnPosition = stackParent.position;
            if (collectedBoxes.Count > 0)
            {
                spawnPosition = collectedBoxes[collectedBoxes.Count - 1].position + (Vector3.back * boxSpacingZ);
            }

            boxTransform.position = spawnPosition;
            boxTransform.localRotation = Quaternion.identity;

            collectedBoxes.Add(boxTransform);
            playerData.currentBoxCount = collectedBoxes.Count;
        }

        private void MoveStackedBoxesElastic()
        {
            if (collectedBoxes.Count == 0) return;

            for (int i = 0; i < collectedBoxes.Count; i++)
            {
                Transform currentBox = collectedBoxes[i];
                Transform leadBox = (i == 0) ? stackParent : collectedBoxes[i - 1];

                float targetX = Mathf.Lerp(currentBox.position.x, leadBox.position.x, Time.deltaTime * horizontalFollowSpeed);
                float targetZ = Mathf.Lerp(currentBox.position.z, leadBox.position.z - boxSpacingZ, Time.deltaTime * forwardFollowSpeed);
                
                float fallbackYWithoutOffset = leadBox.position.y - boxHeightOffset;
                float targetY = GetPreciseGroundHeight(targetX, currentBox.position.y, targetZ, fallbackYWithoutOffset);

                currentBox.position = new Vector3(targetX, targetY, targetZ);
                currentBox.rotation = Quaternion.identity;
            }
        }

        private float GetPreciseGroundHeight(float x, float currentY, float z, float fallbackY)
        {
            Vector3 rayOrigin = new Vector3(x, currentY + 2f, z);
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
            {
                return hit.point.y + boxHeightOffset;
            }

            return fallbackY + boxHeightOffset;
        }

        private void ClearAndDestroyStack()
        {
            isStackCleared = true;

            for (int i = 0; i < collectedBoxes.Count; i++)
            {
                if (collectedBoxes[i] != null)
                {
                    Destroy(collectedBoxes[i].gameObject);
                }
            }

            collectedBoxes.Clear();
            playerData.currentBoxCount = 0;
        }

        /// <summary>
        /// Extracts and hands over control of all collected boxes to the finish line sequence.
        /// </summary>
        public List<Transform> TakeStackForWinSequence()
        {
            // Prevent the Update loop from continuing to clear or manage the stack positionally
            isStackCleared = true; 
            
            // Create a shadow copy of the list to return safely
            List<Transform> boxesToAnimate = new List<Transform>(collectedBoxes);
            
            // Clear internal tracking safely without destroying objects
            collectedBoxes.Clear();
            
            return boxesToAnimate;
        }
    }
}