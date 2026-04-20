using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public bool IsDead { get; private set; }

    [Header("Invencibilidade")]
    public float invincibilityTime = 0.5f;
    private bool isInvincible = false;

    [Header("Knockback")]
    public float knockbackTime = 0.2f;
    private bool isKnocked = false;

    public bool IsKnocked => isKnocked;

    private Rigidbody2D rb;
    private HitFlashController flash;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        flash = GetComponentInChildren<HitFlashController>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        Debug.Log("Player tomou dano! Vida: " + currentHealth);

        flash?.Flash();

        StartCoroutine(HandleKnockback(knockback));
        StartCoroutine(Invincibility());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HandleKnockback(Vector2 knockback)
    {
        isKnocked = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        isKnocked = false;
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    void Die()
    {
        IsDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Debug.Log("Player morreu");
    }
}