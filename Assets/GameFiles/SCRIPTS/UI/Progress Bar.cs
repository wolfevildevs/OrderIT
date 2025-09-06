using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    [Header("World References")]
    public Transform player;
    public Transform startWorld;
    public Transform endWorld;

    [Header("UI References")]
    public Text Gameover_txt;
    public RectTransform playerIcon;
    public RectTransform uiStartPoint;
    public RectTransform uiEndPoint;
    public Text progressText;

    private Vector2 uiStartPos;
    private Vector2 uiEndPos;

    void Start()
    {
        uiStartPos = uiStartPoint.anchoredPosition;
        uiEndPos = uiEndPoint.anchoredPosition;
    }

    void Update()
    {
        float totalDistance = Vector3.Distance(startWorld.position, endWorld.position);
        float playerDistance = Vector3.Distance(startWorld.position, player.position);
        float progress = Mathf.Clamp01(playerDistance / totalDistance);

        Vector2 newPos = Vector2.Lerp(uiStartPos, uiEndPos, progress);
        playerIcon.anchoredPosition = newPos;

        int percent = Mathf.RoundToInt(progress * 100f);
        progressText.text = percent + "%";
        Gameover_txt.text = progressText.text;
    }

    public void StopProgress()
    {
        enabled = false;
    }
}