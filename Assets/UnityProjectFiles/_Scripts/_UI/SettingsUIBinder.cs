using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Audio;

namespace RunnerGame.UI
{
    /// <summary>
    /// Binds UI Buttons (On/Off pairs) to the global AudioManager and persistent PlayerPrefs.
    /// Can be attached to multiple panels (e.g., Main Menu Settings, Pause Menu) simultaneously.
    /// </summary>
    public class SettingsUIBinder : MonoBehaviour
    {
        [Header("Music Buttons")]
        [SerializeField] private Button musicOnButton;
        [SerializeField] private Button musicOffButton;

        [Header("SFX Buttons")]
        [SerializeField] private Button sfxOnButton;
        [SerializeField] private Button sfxOffButton;

        [Header("Vibration Buttons")]
        [SerializeField] private Button vibOnButton;
        [SerializeField] private Button vibOffButton;

        private void Start()
        {
            // Bind the explicit On/Off buttons to the system logic mechanically
            if (musicOnButton != null) musicOnButton.onClick.AddListener(() => SetMusicState(true));
            if (musicOffButton != null) musicOffButton.onClick.AddListener(() => SetMusicState(false));

            if (sfxOnButton != null) sfxOnButton.onClick.AddListener(() => SetSFXState(true));
            if (sfxOffButton != null) sfxOffButton.onClick.AddListener(() => SetSFXState(false));

            if (vibOnButton != null) vibOnButton.onClick.AddListener(() => SetVibrationState(true));
            if (vibOffButton != null) vibOffButton.onClick.AddListener(() => SetVibrationState(false));

            // Sync visual states on initialization based on saved preferences
            LoadAndApplySettings();
        }

        /// <summary>
        /// Updates the music state, saves the preference, and refreshes the UI graphics.
        /// </summary>
        private void SetMusicState(bool isOn)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.ToggleMusic(isOn);
            
            PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();

            PlayClickSound();
            UpdateVisualFeedback();
        }

        /// <summary>
        /// Updates the sound effects state, saves the preference, and refreshes the UI graphics.
        /// </summary>
        private void SetSFXState(bool isOn)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.ToggleSFX(isOn);

            PlayerPrefs.SetInt("SFXEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();

            PlayClickSound();
            UpdateVisualFeedback();
        }

        /// <summary>
        /// Updates the device haptic vibration state, saves the preference, and refreshes the UI graphics.
        /// </summary>
        private void SetVibrationState(bool isOn)
        {
            PlayerPrefs.SetInt("VibEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();

            // Trigger a physical device vibration as immediate feedback if turned ON
            if (isOn)
            {
                #if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
                #endif
            }

            PlayClickSound();
            UpdateVisualFeedback();
        }

        /// <summary>
        /// Fetches saved states from PlayerPrefs (Defaults to 1/True) and applies them globally.
        /// </summary>
        private void LoadAndApplySettings()
        {
            bool musicLoaded = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            bool sfxLoaded = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleMusic(musicLoaded);
                AudioManager.Instance.ToggleSFX(sfxLoaded);
            }

            UpdateVisualFeedback();
        }

        /// <summary>
        /// Toggles the active state (SetActive) of the buttons to show only the correct option.
        /// </summary>
        private void UpdateVisualFeedback()
        {
            // Sync Music buttons
            bool musicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            // لو الموسيقى شغال، اخفي زرار الـ On واظهر زرار الـ Off والعكس بالعكس
            if (musicOnButton != null) musicOnButton.gameObject.SetActive(!musicOn);
            if (musicOffButton != null) musicOffButton.gameObject.SetActive(musicOn);

            // Sync SFX buttons
            bool sfxOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
            if (sfxOnButton != null) sfxOnButton.gameObject.SetActive(!sfxOn);
            if (sfxOffButton != null) sfxOffButton.gameObject.SetActive(sfxOn);

            // Sync Vibration buttons
            bool vibOn = PlayerPrefs.GetInt("VibEnabled", 1) == 1;
            if (vibOnButton != null) vibOnButton.gameObject.SetActive(!vibOn);
            if (vibOffButton != null) vibOffButton.gameObject.SetActive(vibOn);
        }

        /// <summary>
        /// Routes a click sound request to the central Audio Manager securely.
        /// </summary>
        private void PlayClickSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("click");
            }
        }
    }
}