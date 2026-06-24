using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
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

    [Header("HitStop")]
    public float hitStopTime = 0.12f;

    [Header("Morte")]
    public float deathDelay = 0.6f;
    public MonoBehaviour playerControl; // arrasta seu script de movimento aqui

    private Rigidbody2D rb;
    private HitFlashController flash;

    public PlayerSound playerSound;
    public HeartUI heartUI;

    void Awake()
    {
        currentHealth = maxHealth;
        heartUI?.UpdateHearts(currentHealth);

        rb = GetComponent<Rigidbody2D>();
        flash = GetComponentInChildren<HitFlashController>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isInvincible || IsDead) return;

        currentHealth -= damage;
        heartUI?.UpdateHearts(currentHealth);

        Debug.Log("Player tomou dano! Vida: " + currentHealth);

        playerSound?.PlayHit();
        flash?.Flash();

        StartCoroutine(HitStop());
        StartCoroutine(HandleKnockback(knockback));
        StartCoroutine(Invincibility());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Cura o jogador até a vida máxima
    public void HealToMax()
    {
        if (IsDead) return;

        currentHealth = maxHealth;
        heartUI?.UpdateHearts(currentHealth);
    }

    IEnumerator HitStop()
    {
        float originalTime = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = originalTime;
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
        if (IsDead) return;

        IsDead = true;

        // trava controle do player
        if (playerControl != null)
            playerControl.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;

        
        rb.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);

        Debug.Log("Player morreu");

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);


    }
}