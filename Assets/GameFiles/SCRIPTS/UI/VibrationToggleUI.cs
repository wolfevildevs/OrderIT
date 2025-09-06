using UnityEngine;
using UnityEngine.UI;

public class VibrationToggleLoader : MonoBehaviour
{
    public Toggle vibrationToggle;

    void Start()
    {
        if (PlayerPrefs.HasKey("VibrationEnabled"))
        {
            bool enabled = PlayerPrefs.GetInt("VibrationEnabled") == 1;
            vibrationToggle.isOn = enabled;
        }
        else
        {
            vibrationToggle.isOn = true;
            PlayerPrefs.SetInt("VibrationEnabled", 1);
            PlayerPrefs.Save();
        }
    }
}
