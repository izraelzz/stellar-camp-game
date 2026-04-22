using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip jump;
    public AudioClip hitDamage;
    public AudioClip[] attacks;
    public AudioClip[] footsteps;
    public AudioClip dash;

    public void PlayJump()
    {
        audioSource.PlayOneShot(jump);
    }

    public void PlayHit()
    {
        audioSource.PlayOneShot(hitDamage);
    }

    public void PlayAttack()
    {
        int index = Random.Range(0, attacks.Length);
        audioSource.PlayOneShot(attacks[index]);
    }

    public void PlayFootstep()
    {
        int index = Random.Range(0, footsteps.Length);
        audioSource.PlayOneShot(footsteps[index], 0.5f);
    }

    public void PlayDash()
{
    audioSource.PlayOneShot(dash);
}
}