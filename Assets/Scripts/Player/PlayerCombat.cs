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

    [Header("Impacto")]
    public float hitStopTime = 0.08f;
    public float knockbackForce = 6f;

    [Header("Controle durante ataque")]
    public float attackMoveLock = 0.3f;

    private PlayerAnimation anim;
    private PlayerMovement movement;

    void Awake()
    {
        anim = GetComponentInChildren<PlayerAnimation>();
        movement = GetComponent<PlayerMovement>();
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
            {
                StartAttack();
            }
            else
            {
                queuedNext = true;
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = comboResetTime;

        StartCoroutine(AttackMovementLock());

        anim.PlayAttack(comboStep);
    }

    IEnumerator AttackMovementLock()
    {
        float originalSpeed = movement.moveSpeed;
        movement.moveSpeed *= attackMoveLock;

        yield return new WaitForSeconds(0.1f);

        movement.moveSpeed = originalSpeed;
    }

    // CHAMADO NA ANIMAÇÃO
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
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(dir * (knockbackForce + comboStep * 2f), ForceMode2D.Impulse);
            }

            // HITSTOP
            StartCoroutine(HitStop());
        }
    }

    IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = 1f;
    }

    // FINAL DA ANIMAÇÃO
    public void EndAttack()
    {
        isAttacking = false;

        if (queuedNext)
        {
            queuedNext = false;
            StartAttack();
        }
        else
        {
            anim.ResetAttack();
        }
    }

    void HandleComboReset()
    {
        if (isAttacking) return;

        comboTimer -= Time.deltaTime;

        if (comboTimer <= 0)
        {
            comboStep = 0;
            anim.ResetAttack();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}