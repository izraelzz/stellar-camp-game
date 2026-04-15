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

    private PlayerAnimation anim;
    private Rigidbody2D rb;

    void Awake()
    {
        anim = GetComponentInChildren<PlayerAnimation>();
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
    }

    void StartAttack()
    {
        isAttacking = true;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = comboResetTime;

        StartCoroutine(AttackLock());

        anim.PlayAttack(comboStep);
    }

    IEnumerator AttackLock()
    {
        float originalGravity = rb.gravityScale;

        // trava movimento horizontal
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        yield return new WaitForSeconds(0.1f);
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