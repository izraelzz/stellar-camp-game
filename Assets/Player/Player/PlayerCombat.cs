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
    public float cancelWindow = 0.15f;
    private float attackStartTime;

    [Header("Impacto")]
    public float hitStopTime = 0.08f;
    public float knockbackForce = 6f;

    private Rigidbody2D rb;

    // 🔥 ATAQUE TRAVADO
    private string currentAttackName;

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

        if (!isAttacking && queuedNext)
        {
            queuedNext = false;
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        // 🔥 só 2 ataques
        comboStep++;
        if (comboStep > 2)
            comboStep = 1;

        comboTimer = comboResetTime;
        attackStartTime = Time.time;

        // 🔥 DECIDE ATAQUE NO CLIQUE
        float vertical = Input.GetAxisRaw("Vertical");
        bool grounded = GetComponent<PlayerMovement>().LastOnGroundTime > 0;

        if (grounded)
        {
            if (vertical > 0.5f)
                currentAttackName = "IdleUpAttack";
            else
                currentAttackName = "Attack" + comboStep;
        }
        else
        {
            if (vertical > 0.5f)
                currentAttackName = "JumpUpAttack";
            else if (vertical < -0.5f)
                currentAttackName = "JumpDownAttack";
            else
                currentAttackName = "Attack1";
        }

        StartCoroutine(AttackLock());
    }

    IEnumerator AttackLock()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);
    }

    public bool TryCancelAttack()
    {
        if (!isAttacking) return false;

        if (Time.time - attackStartTime < cancelWindow)
            return false;

        isAttacking = false;
        return true;
    }

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

    public string GetAttackName()
    {
        return currentAttackName;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}