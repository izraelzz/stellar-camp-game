using UnityEngine;
using System.Collections;

public class BeeController : MonoBehaviour
{
    public Transform player;

    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Detecção")]
    public float detectRange = 7f;

    [Header("Movimento")]
    public float flySpeed = 3.5f;

    [Header("Ataque")]
    public float attackTriggerRange = 4f;
    public float attackCooldown = 1.5f;
    public float dashForce = 14f;
    public float dashDuration = 0.25f;
    public float windupTime = 0.2f;

    [Header("Recover")]
    public float recoverUpForce = 7f;
    public float recoverTime = 0.3f;

    [Header("Knockback")]
    public float knockbackResist = 0.5f;

    [Header("Death Physics")]
    public float deathKnockbackForce = 6f;
    public float deathUpForce = 4f;
    public float deathTorque = 200f;
    public float deathGravity = 2.5f;
    public float deathDestroyTime = 2f;

    private float lastAttackTime;
    private bool isAttacking = false;
    private bool isWindingUp = false;
    private bool isDashing = false;
    private bool isRecovering = false;

    private bool isDead = false;
    private bool isHit = false;
    private bool hasLanded = false;

    private BeeState currentState;

    private Vector2 lockedTargetPosition;

    public enum BeeState
    {
        Idle,
        Chase,
        Attack,
        Hit,
        Death
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (isDead)
        {
            currentState = BeeState.Death;
            return;
        }

        if (player == null)
        {
            currentState = BeeState.Idle;
            return;
        }

        if (isHit)
        {
            currentState = BeeState.Hit;
            return;
        }

        if (isAttacking)
        {
            currentState = BeeState.Attack;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            if (dist <= attackTriggerRange)
            {
                rb.linearVelocity *= 0.8f;
                TryAttack();
                currentState = BeeState.Idle;
            }
            else
            {
                currentState = BeeState.Chase;
            }
        }
        else
        {
            currentState = BeeState.Idle;
        }

        HandleState();
    }

    void HandleState()
    {
        switch (currentState)
        {
            case BeeState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case BeeState.Chase:
                ChasePlayer();
                break;

            case BeeState.Attack:
                break;

            case BeeState.Hit:
                break;

            case BeeState.Death:
                break;
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * flySpeed;

        if (dir.x != 0)
            transform.localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
    }

    void TryAttack()
    {
        if (isAttacking) return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        lockedTargetPosition = player.position;

        isWindingUp = true;
        rb.linearVelocity *= 0.3f;

        yield return new WaitForSeconds(windupTime);
        isWindingUp = false;

        isDashing = true;

        Vector2 rawDir = (lockedTargetPosition - (Vector2)transform.position).normalized;


        float minY = -0.3f;
        if (rawDir.y < minY)
        {
            rawDir.y = minY;
            rawDir = rawDir.normalized;
        }

        FaceDirection(rawDir);

        rb.linearVelocity = rawDir * dashForce;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        isRecovering = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, recoverUpForce);

        yield return new WaitForSeconds(recoverTime);

        isRecovering = false;
        isAttacking = false;
    }


    public void TakeHit(Vector2 knockback)
    {
        if (isDead) return;

        isHit = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback * (1f - knockbackResist), ForceMode2D.Impulse);

        Invoke(nameof(RecoverFromHit), 0.2f);
    }

    void RecoverFromHit()
    {
        isHit = false;
    }


    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentState = BeeState.Death;

        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = deathGravity;
        rb.freezeRotation = false;

        float dir = 1f;
        if (player != null)
            dir = Mathf.Sign(transform.position.x - player.position.x);

        Vector2 force = new Vector2(dir * deathKnockbackForce, deathUpForce);
        rb.AddForce(force, ForceMode2D.Impulse);

        rb.AddTorque(-dir * deathTorque);

        col.enabled = false;

        Destroy(gameObject, deathDestroyTime);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead || hasLanded) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.freezeRotation = true;
        }
    }


    public BeeState GetState() => currentState;
    public bool IsDead() => isDead;
    public bool IsDashing() => isDashing;
    public bool IsWindingUp() => isWindingUp;
    public bool IsRecovering() => isRecovering;

    void FaceDirection(Vector2 dir)
    {
        if (dir.x == 0) return;
        transform.localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
    }
}