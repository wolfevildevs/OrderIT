using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject PauseMenu;

    public void Pause_Button()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume_Button()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
