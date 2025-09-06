using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlButton : MonoBehaviour
{
    public string url;

    public void OpenIt()
    {
        Application.OpenURL(url);
    }
}
