using UnityEngine;

public class MobSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip hit;
    public AudioClip death;
    public AudioClip windup; 

    public void PlayHit()
    {
        audioSource.PlayOneShot(hit);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(death);
    }

    public void PlayWindup()
    {
        audioSource.PlayOneShot(windup);
    }
}