using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndDemoTrigger : MonoBehaviour
{
    public ScreenFade fade;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        StartCoroutine(End());
    }

    IEnumerator End()
    {
        yield return fade.FadeIn();
        SceneManager.LoadScene("EndScene");
    }
}