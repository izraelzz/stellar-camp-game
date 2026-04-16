using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    [Header("Combo")]
    public float comboResetTime = 0.4f;

    private int comboStep = 0;
    private float comboTimer;

    private bool isAttacking = false;
    private bool queuedNext = false;

    [Header("Cancel")]
    public float cancelWindow = 0.15f; // tempo antes de poder cancelar
    private float attackStartTime;

    [Header("Impacto")]
    public float hitStopTime = 0.08f;
    public float knockbackForce = 6f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        HandleComboReset();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
                StartAttack();
            else
                queuedNext = true;
        }

        // 🔥 PROCESSA COMBO
        if (!isAttacking && queuedNext)
        {
            queuedNext = false;
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = comboResetTime;
        attackStartTime = Time.time;

        StartCoroutine(AttackLock());
    }

    IEnumerator AttackLock()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);
    }

    // 🔥 CANCEL COM CONTROLE
    public bool TryCancelAttack()
    {
        if (!isAttacking) return false;

        // só cancela depois de um tempo mínimo
        if (Time.time - attackStartTime < cancelWindow)
            return false;

        isAttacking = false;
        return true;
    }

    // 🎬 EVENTO
    public void PerformAttack()
    {
        Vector2 dir = Vector2.right * Mathf.Sign(transform.localScale.x);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();

            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector2.zero;
                enemyRb.AddForce(dir * (knockbackForce + comboStep * 2f), ForceMode2D.Impulse);
            }

            StartCoroutine(HitStop());
        }
    }

    IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = 1f;
    }

    // 🎬 EVENTO FINAL
    public void EndAttack()
    {
        isAttacking = false;
    }

    void HandleComboReset()
    {
        if (isAttacking) return;

        comboTimer -= Time.deltaTime;

        if (comboTimer <= 0)
            comboStep = 0;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public int GetComboStep()
    {
        return comboStep;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}