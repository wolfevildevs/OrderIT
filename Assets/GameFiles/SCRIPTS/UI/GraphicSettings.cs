using UnityEngine;

public class GraphicSettings : MonoBehaviour
{
    public void ChangeQuality(int qualityindex)
    {
        QualitySettings.SetQualityLevel(qualityindex);
    }
}
