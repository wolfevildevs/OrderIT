using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunnerGame.Player;
using RunnerGame.Core;
using RunnerGame.Audio;

namespace RunnerGame.Level
{
    /// <summary>
    /// Intercepts the player bounding collider at the end of the road to trigger the kinematic box flying reward sequence.
    /// </summary>
    public class FinishLineTrigger : MonoBehaviour
    {
        [Header("Data Link")]
        [SerializeField] private PlayerDataSO playerData;

        [Header("Sequence Mechanics")]
        [SerializeField] private Transform boxTargetPoint;
        [SerializeField] private float boxFlySpeed = 15f;
        [SerializeField] private float delayBetweenBoxes = 0.15f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject boxVFXPrefab;

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;

            if (other.CompareTag("Player") || other.GetComponent<PlayerMovementController>() != null)
            {
                hasTriggered = true;
                StartCoroutine(ExecuteWinSequence(other.gameObject));
            }
        }

        private IEnumerator ExecuteWinSequence(GameObject playerObject)
        {
            // Halt live continuous mechanics inputs loop
            if (playerData != null) playerData.isMoving = false;

            // Stabilize and freeze player physics instantly to trigger automatic animation Idle switch
            if (playerObject.TryGetComponent<Rigidbody>(out Rigidbody playerRb))
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.isKinematic = true; 
            }

            // Extract the shadow list of transforms from the stacking subsystem
            if (playerObject.TryGetComponent<PlayerStackManager>(out PlayerStackManager stackManager))
            {
                List<Transform> boxesToAnimate = stackManager.TakeStackForWinSequence();

                int dynamicAccumulatedScore = 0;
                int dynamicAccumulatedCurrency = 0;

                // Animate boxes backwards from top/back of the stack layout
                for (int i = boxesToAnimate.Count - 1; i >= 0; i--)
                {
                    Transform currentBox = boxesToAnimate[i];
                    if (currentBox == null) continue;

                    yield return StartCoroutine(AnimateBoxToTarget(currentBox));

                    // Generate random score reward per box bounded between 10 and 100 points
                    int randomScoreReward = Random.Range(10, 101);
                    dynamicAccumulatedScore += randomScoreReward;

                    // Standard currency conversion metric calculation layout (100 pts = $5 -> Ratio 0.05)
                    float calculatedCoins = randomScoreReward * 0.05f;
                    dynamicAccumulatedCurrency += Mathf.CeilToInt(calculatedCoins);

                    // Spawn visual boom explosion particles upon target arrival
                    if (boxVFXPrefab != null && boxTargetPoint != null)
                    {
                        Instantiate(boxVFXPrefab, boxTargetPoint.position, Quaternion.identity);
                        AudioManager.Instance.PlaySFX("box");
                    }

                    Destroy(currentBox.gameObject);

                    // Tiny cinematic pacing gap before pulling the next block
                    yield return new WaitForSeconds(delayBetweenBoxes);
                }

                // Dispatch finalized values back to core game loop architecture handshake
                GameManager.Instance.CompleteLevel(dynamicAccumulatedScore, dynamicAccumulatedCurrency);
            }
            else
            {
                // Absolute structural fallback protection
                GameManager.Instance.CompleteLevel(100, 5);
            }
        }

        private IEnumerator AnimateBoxToTarget(Transform boxTransform)
        {
            if (boxTargetPoint == null || boxTransform == null) yield break;

            while (boxTransform != null && Vector3.Distance(boxTransform.position, boxTargetPoint.position) > 0.1f)
            {
                boxTransform.position = Vector3.MoveTowards(boxTransform.position, boxTargetPoint.position, boxFlySpeed * Time.deltaTime);
                boxTransform.Rotate(Vector3.up * 360f * Time.deltaTime); 
                yield return null;
            }
        }
    }
}