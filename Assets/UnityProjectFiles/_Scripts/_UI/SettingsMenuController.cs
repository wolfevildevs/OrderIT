using UnityEngine;
using UnityEngine.UI;

namespace RunnerGame.UI
{
    /// <summary>
    /// Commits and manages persistent user peripheral choices like sound or haptic feedback states.
    /// </summary>
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("UI Toggle Buttons Layout")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle hapticToggle;

        private const string SoundPrefKey = "Settings_SoundEnabled";
        private const string HapticPrefKey = "Settings_HapticEnabled";

        private void Start()
        {
            LoadUserSystemSettings();

            if (soundToggle != null) soundToggle.onValueChanged.AddListener(SetSoundState);
            if (hapticToggle != null) hapticToggle.onValueChanged.AddListener(SetHapticState);
        }

        private void LoadUserSystemSettings()
        {
            // Fallback default value is 1 (True / Enabled)
            bool isSoundOn = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;
            bool isHapticOn = PlayerPrefs.GetInt(HapticPrefKey, 1) == 1;

            if (soundToggle != null) soundToggle.isOn = isSoundOn;
            if (hapticToggle != null) hapticToggle.isOn = isHapticOn;

            ApplyAudioSystemStates(isSoundOn);
        }

        public void SetSoundState(bool isEnabled)
        {
            PlayerPrefs.SetInt(SoundPrefKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplyAudioSystemStates(isEnabled);
        }

        public void SetHapticState(bool isEnabled)
        {
            PlayerPrefs.SetInt(HapticPrefKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            
            if (isEnabled)
            {
                // Trigger casual device vibration feedback if on supported mobile devices
#if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
#endif
            }
        }

        private void ApplyAudioSystemStates(bool isSoundOn)
        {
            // Mute or Unmute the Master Audio Listener inside Unity Engine globally
            AudioListener.pause = !isSoundOn;
            Debug.Log($"Master Audio State update: Sound Active = {isSoundOn}");
        }
    }
}