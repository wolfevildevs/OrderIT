using UnityEngine;
using UnityEngine.UI;

namespace RunnerGame.UI
{
    /// <summary>
    /// Responsible for opening a specific external URL in the device's default browser when the button is clicked.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class URLOpener : MonoBehaviour
    {
        [Header("URL Settings")]
        [Tooltip("Enter the full URL here (e.g., https://www.google.com)")]
        [SerializeField] private string targetUrl;

        private Button button;

        private void Awake()
        {
            // Automatically get the Button component attached to this GameObject
            button = GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError($"[{nameof(URLOpener)}] No Button component found on this GameObject: {gameObject.name}");
                return;
            }

            // Bind the click event to the URL opening logic
            button.onClick.AddListener(OpenTargetURL);
        }

        /// <summary>
        /// Opens the configured URL in the system's default web browser after validation.
        /// </summary>
        public void OpenTargetURL()
        {
            if (string.IsNullOrEmpty(targetUrl))
            {
                Debug.LogWarning($"[{nameof(URLOpener)}] Target URL is empty! Please assign a valid URL in the Inspector for: {gameObject.name}");
                return;
            }

            // Open the URL securely in the external browser
            Application.OpenURL(targetUrl.Trim());
        }
    }
}