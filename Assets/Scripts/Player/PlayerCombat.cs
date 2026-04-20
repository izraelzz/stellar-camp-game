using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    public Transform attackPoint;
    public Transform attackPointUp;
    public Transform attackPointDown;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float attackCooldown = 0f;

    [Header("Combo")]
    public float comboResetTime = 0.4f;

    private int comboStep = 0;
    private float comboTimer;
    private float lastAttackTime = 0f;
    private Transform currentAttackPoint;

    private bool isAttacking = false;
    private bool queuedNext = false;
    private bool hasBouncedThisAttack = false;

    [Header("Impacto")]
    public float hitStopTime = 0.08f;
    public float knockbackForce = 6f;
    public float pogoKnockbackForce = 2f;

    [Header("Bounce (Pogo)")]
    public float bounceForce = 10f;
    public float bounceResetY = 0f;

    private Rigidbody2D rb;
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
        if (lastAttackTime > 0 && Time.time < lastAttackTime + attackCooldown)
            return;

        isAttacking = true;
        hasBouncedThisAttack = false;
        lastAttackTime = Time.time;

        comboStep++;
        if (comboStep > 2)
            comboStep = 1;

        comboTimer = comboResetTime;

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

        if (vertical > 0.5f)
            currentAttackPoint = attackPointUp;
        else if (vertical < -0.5f)
            currentAttackPoint = attackPointDown;
        else
            currentAttackPoint = attackPoint;

        StartCoroutine(AttackLock());
    }

    IEnumerator AttackLock()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);
    }

    void ApplyBounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceResetY);
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
    }

    public void PerformAttack()
    {
        Transform activePoint = currentAttackPoint != null ? currentAttackPoint : attackPoint;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            activePoint.position,
            attackRange,
            enemyLayer
        );

        bool hitSomething = false;
        HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            if (hitEnemies.Contains(hit.gameObject)) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                Vector2 dir;

                if (currentAttackName == "JumpUpAttack" || currentAttackName == "IdleUpAttack")
                {
    
                    float x = Mathf.Sign(transform.localScale.x) * 0.3f;
                    dir = new Vector2(x, 1f).normalized;
                }
                else if (currentAttackName == "JumpDownAttack")
                {
                    dir = Vector2.down;
                }
                else
                {
                    dir = Vector2.right * Mathf.Sign(transform.localScale.x);
                }

                float currentKnockback = (currentAttackName == "JumpDownAttack")
                    ? pogoKnockbackForce
                    : (knockbackForce + comboStep * 2f);

                Vector2 knockback = dir * currentKnockback;

                damageable.TakeDamage(damage, knockback);

                hitEnemies.Add(hit.gameObject);
                hitSomething = true;

                HandleBounce(hit.transform);
            }
        }

        if (hitSomething)
        {
            StartCoroutine(HitStop());
            CameraShake2D.Instance?.Shake(0.1f, 1.2f, 15f);
        }
    }

    void HandleBounce(Transform enemy)
    {
        bool isEnemyBelow = enemy.position.y < transform.position.y - 0.1f;

        if (currentAttackName == "JumpDownAttack"
            && isEnemyBelow
            && !hasBouncedThisAttack
            && rb.linearVelocity.y <= 0)
        {
            ApplyBounce();
            hasBouncedThisAttack = true;
        }
    }

    IEnumerator HitStop()
    {
        float originalTime = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = originalTime;
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
    Gizmos.color = Color.red;

    if (attackPoint != null)
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    if (attackPointUp != null)
        Gizmos.DrawWireSphere(attackPointUp.position, attackRange);

    if (attackPointDown != null)
        Gizmos.DrawWireSphere(attackPointDown.position, attackRange);
}
}