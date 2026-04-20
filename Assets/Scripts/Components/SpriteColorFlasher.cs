using System.Collections;
using UnityEngine;

public class SpriteColorFlasher : MonoBehaviour
{
    private Coroutine currentFlash;

    public void FlashColor(SpriteRenderer spriteRend, float duration, Color color)
    {
        if (currentFlash != null)
            StopCoroutine(currentFlash);

        currentFlash = StartCoroutine(DoColorFlash(spriteRend, duration, color));
    }

    private IEnumerator DoColorFlash(SpriteRenderer spriteRend, float duration, Color newColor)
    {
        if (spriteRend == null) yield break;

        Color oldColor = spriteRend.color;

        spriteRend.color = newColor;

        yield return new WaitForSeconds(duration);

        if (spriteRend != null)
            spriteRend.color = oldColor;

        currentFlash = null;
    }
}