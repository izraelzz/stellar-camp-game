using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;
    public float speed = 2f;

    public IEnumerator FadeIn()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }
}