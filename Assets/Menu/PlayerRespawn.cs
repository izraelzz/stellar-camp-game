using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public ScreenFade fade;

    public void Die()
    {
        StartCoroutine(RestartScene());
    }

    IEnumerator RestartScene()
    {
       
        if (fade != null)
            yield return fade.FadeIn();

       
        yield return new WaitForSeconds(2.0f);

       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}