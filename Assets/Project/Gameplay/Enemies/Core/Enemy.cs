using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable
{
    #region COMPONENTS

    private Rigidbody2D rb;
    private Collider2D col;

    private HitFlashController flash;
    public MobSound mobSound;

    #endregion

    #region TARGET

    public Transform player;

    #endregion

    #region HEALTH

    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead;

    #endregion

    #region MOVEMENT

    [Header("Movement")]
    public float moveSpeed = 2f;

    #endregion

    #region AI SETTINGS

    [Header("AI")]
    public float detectRange = 6f;
    public float attackRange = 1.5f;

    private bool isAgro;
    private bool isAgroStarting;

    #endregion

    #region PATROL

    [Header("Patrol")]
    public float patrolSpeed = 1.2f;
    public float patrolDistance = 2f;
    public float patrolWaitTime = 1.5f;

    private Vector2 startPos;
    private int patrolDir = 1;
    private bool isWaiting;
    private float patrolTimer;

    #endregion

    #region ATTACK

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public float attackWindup = 0.4f;

    private float lastAttackTime;
    private bool isAttacking;
    private bool isWindingUp;

    public Transform attackPoint;
    public float attackRangeCircle = 1.2f;
    public LayerMask playerLayer;

    public int damage = 1;
    public float knockbackForce = 6f;
    public float knockbackUp = 3f;

    #endregion

    #region CONTACT DAMAGE

    public int contactDamage = 1;
    public float contactCooldown = 0.5f;
    private float lastContactTime;

    #endregion

    #region STATE MACHINE

    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hit,
        Death
    }

    private State state;

    private bool isHit;

    #endregion

    #region GETTERS

    public State GetCurrentState()
    {
    return state;
    }

    #endregion


    #region UNITY


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        flash = GetComponentInChildren<HitFlashController>();

        currentHealth = maxHealth;
        startPos = transform.position;

        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isDead)
        {
            state = State.Death;
            return;
        }

        if (player == null)
        {
            state = State.Patrol;
            HandleState();
            return;
        }

        DecideState();
        HandleState();
    }

    #endregion

    #region STATE MACHINE

    void DecideState()
    {
        if (isHit)
        {
            state = State.Hit;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            state = State.Attack;
            TryAttack();
        }
        else if (dist <= detectRange)
        {
            state = State.Chase;
        }
        else
        {
            state = State.Patrol;
        }
    }

    void HandleState()
    {
        switch (state)
        {
            case State.Idle:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case State.Hit:
                break;

            case State.Death:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    #endregion

    #region MOVEMENT LOGIC

    void Patrol()
    {
        float left = startPos.x - patrolDistance;
        float right = startPos.x + patrolDistance;

        if (isWaiting)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0;
                isWaiting = false;
                patrolDir *= -1;
            }

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(patrolDir * patrolSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(-patrolDir, 1, 1);

        if ((patrolDir == 1 && transform.position.x >= right) ||
            (patrolDir == -1 && transform.position.x <= left))
        {
            isWaiting = true;
        }
    }

    void Chase()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(-dir, 1, 1);
    }

    #endregion

    #region ATTACK

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

        PerformAttack();
    }

    void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRangeCircle,
            playerLayer
        );

        HashSet<GameObject> hitPlayers = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            if (hitPlayers.Contains(hit.gameObject)) continue;

            PlayerHealth player = hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
                Vector2 knockback = new Vector2(dir * knockbackForce, knockbackUp);

                player.TakeDamage(damage, knockback);
                hitPlayers.Add(hit.gameObject);
            }
        }
    }

    public void Animation_PerformAttack()
    {
    PerformAttack();
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    #endregion

    #region DAMAGE

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isDead) return;

        currentHealth -= damage;

        mobSound?.PlayHit();
        flash?.Flash();

        TakeHit(knockback);

        if (currentHealth <= 0)
            Die();
    }

    public void TakeHit(Vector2 knockback)
    {
        isHit = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        Invoke(nameof(RecoverFromHit), 0.25f);
    }

    void RecoverFromHit()
    {
        isHit = false;
    }

    #endregion

    #region DEATH

    void Die()
    {
        if (isDead) return;

        isDead = true;
        mobSound?.PlayDeath();

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

    public bool IsDead() => isDead;

    #endregion

    #region GIZMOS

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRangeCircle);
        }
    }

    #endregion
}