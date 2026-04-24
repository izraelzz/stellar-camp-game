using UnityEngine;
using System.Collections;

public class GameStartFade : MonoBehaviour
{
    public ScreenFade fade;

    IEnumerator Start()
    {
       
        if (fade != null && fade.fadeImage != null)
        {
            Color c = fade.fadeImage.color;
            fade.fadeImage.color = new Color(c.r, c.g, c.b, 1f);
        }

        yield return fade.FadeOut(); 
    }
}