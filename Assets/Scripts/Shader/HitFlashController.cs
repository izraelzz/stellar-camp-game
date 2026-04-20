using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class HitFlashController : MonoBehaviour
{
    private Material mat;
    private Coroutine flashRoutine;

    [Header("Flash Settings")]
    public float duration = 0.2f;

    [Tooltip("Curva do efeito (X = tempo, Y = intensidade)")]
    public AnimationCurve flashCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.1f, 1),   // sobe rápido
        new Keyframe(1, 0)       // desce suave
    );

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        mat = sr.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float time = 0;

        while (time < duration)
        {
            float t = time / duration;

            float strength = flashCurve.Evaluate(t);

            mat.SetFloat("_HitStrength", strength);

            time += Time.deltaTime;
            yield return null;
        }

        // garante reset
        mat.SetFloat("_HitStrength", 0);
    }
}