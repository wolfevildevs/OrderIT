using UnityEngine;
using RunnerGame.Player;

namespace RunnerGame.Obstacles
{
    /// <summary>
    /// A lethal bomb obstacle that detonates instantly upon player contact.
    /// Spawns a dramatic explosion visual effect (VFX) and triggers immediate player death.
    /// </summary>
    public class BombObstacle : MonoBehaviour
    {
        [Header("Visual Effects (VFX)")]
        [Tooltip("Assign the explosion particle system prefab here.")]
        [SerializeField] private GameObject explosionVFXPrefab;

        [Header("Audio Settings (Optional)")]
        [Tooltip("Assign an optional audio prefab or clip if you want a sound effect on detonation.")]
        [SerializeField] private AudioClip explosionSound;

        private bool hasDetonated = false;

        private void OnCollisionEnter(Collision collision)
        {
            // Trigger detonation matrix if colliding with the player
            if (collision.gameObject.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                Detonate(player);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Fallback check in case the developer configures the bomb collider as a Trigger
            if (other.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                Detonate(player);
            }
        }

        /// <summary>
        /// Executes the atomic sequence of the trap: Spawns VFX, kills the player, and cleanly removes the bomb grid.
        /// </summary>
        private void Detonate(PlayerMovementController player)
        {
            // Safety gate to prevent double-detonation bugs in a single frame
            if (hasDetonated) return;
            hasDetonated = true;

            Debug.Log("<color=red>[Bomb Trap] BOOM! Detonating bomb obstacle.</color>");

            // --- 1. SPAWN EXPLOSION VISUAL EFFECT ---
            if (explosionVFXPrefab != null)
            {
                // Instantiate the visual fire/smoke particles exactly at the bomb's world position
                GameObject vfxInstance = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
                
                // Automatically clean up the spawned particle instance after 3 seconds to save memory
                Destroy(vfxInstance, 3f);
            }

            // --- 2. PLAY SOUND EFFECT (Optional) ---
            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            }

            // --- 3. KILL THE PLAYER INSTANTLY ---
            player.Die();

            // --- 4. VANISH THE BOMB FROM THE SCENE ---
            // We destroy the bomb game object immediately so it vanishes from the player's view during the smoke
            Destroy(gameObject);
        }
    }
}