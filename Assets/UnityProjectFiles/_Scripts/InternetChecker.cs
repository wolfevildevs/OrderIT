using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class InternetChecker : MonoBehaviour
{
    public GameObject lockScreenUI; // أربطه من الـInspector
    public float checkInterval = 1.5f; // الفترة الزمنية بين كل فحص (ثواني)

    private Button retryButton;
    private static InternetChecker instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        retryButton = lockScreenUI.GetComponentInChildren<Button>();
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() =>
            {
                StartCoroutine(CheckInternetConnection());
            });
        }

        // Start initial check and looping recheck
        StartCoroutine(CheckInternetConnectionLoop());
    }

    IEnumerator CheckInternetConnectionLoop()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternetConnection());
            yield return new WaitForSeconds(checkInterval);
        }
    }

    IEnumerator CheckInternetConnection()
    {
        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("No Internet Connection");
            if (lockScreenUI != null) lockScreenUI.SetActive(true);
        }
        else
        {
            Debug.Log("Internet Connected");
            if (lockScreenUI != null) lockScreenUI.SetActive(false);
        }
    }
}