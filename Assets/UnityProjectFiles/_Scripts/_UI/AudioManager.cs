using UnityEngine;

namespace RunnerGame.Audio
{
    /// <summary>
    /// Global centralized manager handling Sound Effects and Background Music.
    /// Operates as a Singleton to be accessible from any script in the game.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Player SFX Clips")]
        [SerializeField] private AudioClip playerDeathClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip bombExplosionClip;
        [SerializeField] private AudioClip hammerHitClip;
        [SerializeField] private AudioClip collectBoxClip;
        [SerializeField] private AudioClip levelWinClip;
        [SerializeField] private AudioClip levelLoseClip;
        [SerializeField] private AudioClip buyClip;
        [SerializeField] private AudioClip noMoneyhClip;


        private void Awake()
        {
            // Enforce the Singleton pattern to ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist audio across scene transitions
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Toggles the background music state.
        /// </summary>
        public void ToggleMusic(bool state)
        {
            if (musicSource == null) return;
            musicSource.mute = !state;
        }

        /// <summary>
        /// Toggles the global sound effects state.
        /// </summary>
        public void ToggleSFX(bool state)
        {
            if (sfxSource == null) return;
            sfxSource.mute = !state;
        }

        /// <summary>
        /// Centralized method to play any predefined sound effect using a string identifier.
        /// </summary>
        /// <param name="clipName">The specific name or category of the SFX to play (e.g., "death", "click").</param>
        public void PlaySFX(string clipName)
        {
            if (sfxSource == null) return;

            AudioClip clipToPlay = null;

            // Route the requested string to the exact AudioClip reference safely
            switch (clipName.ToLower())
            {
                case "death":
                case "playerdeath":
                    clipToPlay = playerDeathClip;
                    break;
                case "click":
                case "button":
                    clipToPlay = buttonClickClip;
                    break;
                case "explosion":
                case "bomb":
                    clipToPlay = bombExplosionClip;
                    break;
                case "hammer":
                case "hit":
                    clipToPlay = hammerHitClip;
                    break;
                case "box":
                case "collect":
                    clipToPlay = collectBoxClip;
                    break;
                case "win":
                    clipToPlay = levelWinClip;
                    break;
                case "lose":
                case "levellose":
                    clipToPlay = levelLoseClip;
                    break;
                case "buy":
                case "purchase":
                    clipToPlay = buyClip;
                    break;
                case "nomoney":
                case "nomoneyh":
                    clipToPlay = noMoneyhClip;
                    break;
                default:
                    Debug.LogWarning($"AudioManager: SFX named '{clipName}' is not defined in the system!");
                    break;
            }

            // Play the routed clip if successfully matched
            if (clipToPlay != null)
            {
                sfxSource.PlayOneShot(clipToPlay);
            }
        }
    }
}
//AudioManager.Instance.PlaySFX("");