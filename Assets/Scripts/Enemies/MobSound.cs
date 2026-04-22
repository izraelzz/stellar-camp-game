using UnityEngine;

public class MobSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip hit;
    public AudioClip death;

    public void PlayHit()
    {
        audioSource.PlayOneShot(hit);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(death);
    }
}