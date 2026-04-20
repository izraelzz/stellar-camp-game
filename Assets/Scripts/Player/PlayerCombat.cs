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

    [Header("Cancel")]
    public float cancelWindow = 0.15f;
    private float attackStartTime;

    [Header("Impacto")]
    public float hitStopTime = 0.08f;
    public float knockbackForce = 6f;
    public float pogoKnockbackForce = 2f;

    [Header("Bounce (Pogo)")]
    public float bounceForce = 10f;
    public float bounceResetY = 0f;
    public float bounceCooldown = 0.15f;
    private float lastBounceTime = 0f;

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

    void ApplyBounce()
    {
        lastBounceTime = Time.time;
        // Zera velocidade vertical pra ficar consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceResetY);
        // Aplica impulso pra cima
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
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
        // Verificar cooldown entre ataques
        if (lastAttackTime > 0 && Time.time < lastAttackTime + attackCooldown)
            return;

        isAttacking = true;
        hasBouncedThisAttack = false; // 🔥 reset bounce
        lastAttackTime = Time.time;

        comboStep++;
        if (comboStep > 2)
            comboStep = 1;

        comboTimer = comboResetTime;
        attackStartTime = Time.time;

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

        // Selecionar o attack point correto baseado na direção
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

    public void PerformAttack()
    {
        // Usar o attack point correto para a direção do ataque
        Transform activePoint = currentAttackPoint != null ? currentAttackPoint : attackPoint;
        
        Vector2 dir = Vector2.right * Mathf.Sign(transform.localScale.x);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            activePoint.position,
            attackRange,
            enemyLayer
        );

        bool hitSomething = false;

        // 🔥 evita duplicar hit no mesmo inimigo
        HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            if (hitEnemies.Contains(hit.gameObject)) continue;

            SlimeHealth slime = hit.GetComponent<SlimeHealth>();

            if (slime != null)
            {
                // 🔥 Knockback menor no pogo
                float currentKnockback = (currentAttackName == "JumpDownAttack") ? pogoKnockbackForce : (knockbackForce + comboStep * 2f);
                Vector2 knockback = dir * currentKnockback;

                slime.TakeDamage(damage, knockback);
                hitEnemies.Add(hit.gameObject);
                hitSomething = true;

                // 🔥 POGO SÓ se inimigo estiver ABAIXO do player
                bool isEnemyBelow = hit.transform.position.y < transform.position.y - 0.1f;
                if (currentAttackName == "JumpDownAttack" && isEnemyBelow && !hasBouncedThisAttack && rb.linearVelocity.y <= 0)
                {
                    ApplyBounce();
                    hasBouncedThisAttack = true;
                }
            }
        }

        if (hitSomething)
        {
            StartCoroutine(HitStop());

            // 🎥 CAMERA SHAKE escalável
            float strength = 1.2f + comboStep * 0.8f;
            float duration = 0.08f + comboStep * 0.04f;

            CameraShake2D.Instance?.Shake(0.1f, 1.2f, 15f);
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
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}