using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartEffect : MonoBehaviour
{
    private Image img;

    public float fadeDuration = 0.25f;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void PlayLoseEffect()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Color startColor = img.color;

        while (t < fadeDuration)
        {
            float lerp = t / fadeDuration;

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, lerp);
            img.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        
        Color final = img.color;
        final.a = 0f;
        img.color = final;

        gameObject.SetActive(false);
    }
}