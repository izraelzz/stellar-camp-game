using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartEffect : MonoBehaviour
{
    private Image img;

    public float fadeDuration = 0.25f;
    [Header("Gain Effect")]
    public float gainDuration = 0.35f;
    public float gainOvershoot = 1.2f;
    public float gainStartScale = 0.6f;
    public AudioClip gainSound;
    public ParticleSystem gainParticles;

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

    // restaura o coração (visível e com alpha 1)
    public void Restore()
    {
        StopAllCoroutines();
        if (img == null) img = GetComponent<Image>();
        Color c = img.color;
        c.a = 1f;
        img.color = c;
        gameObject.SetActive(true);
    }

    public void PlayGainEffect()
    {
        StopAllCoroutines();
        if (img == null) img = GetComponent<Image>();
        gameObject.SetActive(true);
        StartCoroutine(GainRoutine());
    }

    IEnumerator GainRoutine()
    {
        float t = 0f;
        // prepare
        transform.localScale = Vector3.one * gainStartScale;
        Color startColor = img.color;
        startColor.a = 0f;
        img.color = startColor;

        if (gainParticles != null)
        {
            gainParticles.Play();
        }

        if (gainSound != null)
        {
            if (Camera.main != null)
                AudioSource.PlayClipAtPoint(gainSound, Camera.main.transform.position);
            else
                AudioSource.PlayClipAtPoint(gainSound, Vector3.zero);
        }

        while (t < gainDuration)
        {
            float progress = t / gainDuration;

            // scale with overshoot (ease out-back like)
            float scale = Mathf.Lerp(gainStartScale, gainOvershoot, Mathf.Sin(progress * (Mathf.PI * 0.5f)));
            transform.localScale = Vector3.one * scale;

            // fade in alpha
            Color c = img.color;
            c.a = Mathf.Lerp(0f, 1f, progress);
            img.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        Color final = img.color;
        final.a = 1f;
        img.color = final;
    }
}