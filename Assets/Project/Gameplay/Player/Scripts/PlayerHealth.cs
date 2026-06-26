using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

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
    public MonoBehaviour playerControl;

    private Rigidbody2D rb;
    private HitFlashController flash;

    public PlayerSound playerSound;
    public HeartUI heartUI;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        flash = GetComponentInChildren<HitFlashController>();

        currentHealth = maxHealth;

        isInvincible = false;
        isKnocked = false;
        IsDead = false;

        heartUI?.UpdateHearts(currentHealth);
    }

    void Start()
    {
        StartCoroutine(ApplyCheckpointRoutine());
    }

    IEnumerator ApplyCheckpointRoutine()
    {
        yield return null; // espera 1 frame

        ApplyCheckpoint();
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

        Debug.Log("Player morreu");

        if (playerControl != null)
            playerControl.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

        StartCoroutine(DeathRoutine());
    }

    IEnumerator RestoreCamera(CinemachineCamera cam)
    {
        var brain = Camera.main.GetComponent<CinemachineBrain>();

        var oldBlend = brain.DefaultBlend;

        // Faz um corte instantâneo
        brain.DefaultBlend = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.Cut, 0f);

        CameraManager.SwitchCamera(cam);

        // Espera um frame
        yield return null;

        // Volta para o blend normal
        brain.DefaultBlend = oldBlend;
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ApplyCheckpoint()
    {
        if (CheckpointManager.Instance == null) return;
        if (!CheckpointManager.Instance.hasCheckpoint) return;

        transform.position = CheckpointManager.Instance.lastCheckpointPosition + Vector3.up * 0.5f;

        Debug.Log("Câmera a restaurar: " + CheckpointManager.Instance.lastCameraName);

        CinemachineCamera[] cameras =
            FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        foreach (var cam in cameras)
        {
            Debug.Log("Encontrada: " + cam.name);

            if (cam.name == CheckpointManager.Instance.lastCameraName)
            {
                Debug.Log("Trocando para: " + cam.name);
                StartCoroutine(RestoreCamera(cam));
                break;
            }
        }
    }
}