using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour
{
    public Transform player;

    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public float detectRange = 6f;
    public float attackRange = 1.5f;

    [Header("Agro Delay")]
    public float agroDelay = 0.5f;
    private bool isAgro = false;
    private bool isAgroStarting = false;

    [Header("Patrol")]
    public float patrolSpeed = 1.2f;
    public float patrolDistance = 2f;
    public float patrolWaitTime = 1.5f;

    private float patrolTimer;
    private int patrolDir = 1;
    private Vector2 startPos;
    private bool isWaiting = false;

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public float attackWindup = 0.4f;

    private float lastAttackTime;
    private bool isWindingUp = false;

    [Header("Contact Damage")]
    public int contactDamage = 1;
    public float contactKnockback = 5f;
    public float contactKnockbackUp = 3f;
    public float contactKnockbackSide = 5f;
    public float contactKnockbackSideDown = 2f;
    public float contactCooldown = 0.5f;
    private float lastContactTime;

    private SlimeState currentState;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isHit = false;

    public enum SlimeState
    {
        Idle,
        Patrol,
        Agro,
        Chase,
        Attack,
        Hit,
        Death
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.freezeRotation = true;
        startPos = transform.position;
    }

    void Update()
    {
        // 🔥 MORTE
        if (isDead)
        {
            currentState = SlimeState.Death;
            return;
        }

        // 🔥 PLAYER SUMIU
        if (player == null)
        {
            ForceIdle();
            return;
        }

        // 🔥 PLAYER MORREU → perde target
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null && ph.IsDead)
        {
            player = null;
            ForceIdle();
            return;
        }

        DecideState();
        HandleState();
    }

    void ForceIdle()
    {
        currentState = SlimeState.Idle;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // 🔥 RESET TOTAL
        isAgro = false;
        isAgroStarting = false;
        isAttacking = false;
        isWindingUp = false;
    }

    void DecideState()
    {
        if (player == null)
        {
            currentState = SlimeState.Patrol;
            return;
        }

        if (isHit)
        {
            currentState = SlimeState.Hit;
            return;
        }

        if (isAttacking)
        {
            currentState = SlimeState.Attack;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            if (!isAgro && !isAgroStarting)
                StartCoroutine(StartAgro());

            if (isAgro)
            {
                if (dist <= attackRange)
                {
                    TryAttack();
                    currentState = SlimeState.Idle;
                }
                else
                {
                    currentState = SlimeState.Chase;
                }
                return;
            }

            currentState = SlimeState.Agro;
            return;
        }

        currentState = SlimeState.Patrol;
    }

    void HandleState()
    {
        switch (currentState)
        {
            case SlimeState.Idle:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case SlimeState.Patrol:
                Patrol();
                break;

            case SlimeState.Chase:
                MoveToPlayer();
                break;

            case SlimeState.Attack:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case SlimeState.Hit:
                // mantém knockback
                break;

            case SlimeState.Death:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    // =========================
    // 🚶 PATROL
    // =========================
    void Patrol()
    {
        float leftLimit = startPos.x - patrolDistance;
        float rightLimit = startPos.x + patrolDistance;

        if (isWaiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0;
                isWaiting = false;
                patrolDir *= -1;
            }

            return;
        }

        rb.linearVelocity = new Vector2(patrolDir * patrolSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(-patrolDir, 1, 1);

        if ((patrolDir == 1 && transform.position.x >= rightLimit) ||
            (patrolDir == -1 && transform.position.x <= leftLimit))
        {
            isWaiting = true;
            patrolTimer = 0;
        }
    }

    // =========================
    // 👁️ AGRO
    // =========================
    IEnumerator StartAgro()
    {
        isAgroStarting = true;
        yield return new WaitForSeconds(agroDelay);
        isAgro = true;
        isAgroStarting = false;
    }

    // =========================
    // 🏃 CHASE
    // =========================
    void MoveToPlayer()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(-dir, 1, 1);
    }

    // =========================
    // ⚔️ ATTACK
    // =========================
    void TryAttack()
    {
        if (isAttacking || isWindingUp) return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isWindingUp = true;
        yield return new WaitForSeconds(attackWindup);
        isWindingUp = false;
        isAttacking = true;
        lastAttackTime = Time.time;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    // =========================
    // 💥 CONTACT DAMAGE
    // =========================
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsDead) return;

        if (Time.time < lastContactTime + contactCooldown)
            return;

        // Verificar se o player está em cima do slime pela posição Y
        float playerY = collision.transform.position.y;
        float slimeY = transform.position.y;
        
        Vector2 knockback;
        
        // Se o player está em cima do slime (posição Y maior), empurrar para o lado
        if (playerY > slimeY + 0.3f)
        {
            // Empurrar para o lado oposto (esquerda ou direita)
            float pushDir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            knockback = new Vector2(pushDir * contactKnockbackSide, -contactKnockbackSideDown);
        }
        else
        {
            // Player na lateral → knockback normal (para trás e para cima)
            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            knockback = new Vector2(dir * contactKnockback, contactKnockbackUp);
        }

        playerHealth.TakeDamage(contactDamage, knockback);

        lastContactTime = Time.time;
    }

    // =========================
    // 💥 HIT
    // =========================
    public void TakeHit(Vector2 knockback)
    {
        if (isDead) return;

        isHit = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        Invoke(nameof(RecoverFromHit), 0.25f);
    }

    void RecoverFromHit()
    {
        isHit = false;
    }

    // =========================
    // 💀 DEATH
    // =========================
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentState = SlimeState.Death;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public SlimeState GetState()
    {
        return currentState;
    }
}