using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float fadeDuration = 0.5f;
    public float visibleTime = 2f;

    private Coroutine fadeRoutine;

    void Start()
    {
        SetAlpha(0f); // começa invisível
    }

    public void ShowScore(int score)
    {
        scoreText.text = "Score: " + score;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // fade IN
        yield return StartCoroutine(Fade(0f, 1f));

        // tempo visível
        yield return new WaitForSeconds(visibleTime);

        // fade OUT
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float start, float end)
    {
        float time = 0;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            float alpha = Mathf.Lerp(start, end, t);

            SetAlpha(alpha);

            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(end);
    }

    void SetAlpha(float alpha)
    {
        Color c = scoreText.color;
        c.a = alpha;
        scoreText.color = c;
    }
}