using UnityEngine;
using RunnerGame.Player;
using RunnerGame.Audio;

namespace RunnerGame.Obstacles
{
    /// <summary>
    /// Instantly triggers the player's defeat script and terminates the active execution run.
    /// </summary>
    public class InstantDeathTrap : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // Check if the object entering the death volume is the player
            if (other.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                Debug.Log("<color=red>[Obstacle] Player hit an instant death hazard!</color>");
                
                // Call the definitive kill function inside player physics layers
                //AudioManager.Instance.PlaySFX("death");
                player.Die();
            }
        }
    }
}