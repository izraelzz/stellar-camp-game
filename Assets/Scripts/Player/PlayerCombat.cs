using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    [Header("Combo")]
   [SerializeField] public float comboResetTime = 0.3f;

    private int comboStep = 0;
    [SerializeField] private float comboTimer;

    private bool isAttacking = false;
    private bool queuedNext = false;

    private PlayerAnimation anim;

    void Awake()
    {
        anim = GetComponentInChildren<PlayerAnimation>();
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

        anim.PlayAttack(comboStep);
    }

    // 🔥 chamado via Animation Event
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
            Debug.Log("HIT combo " + comboStep);

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(dir * (comboStep * 2f), ForceMode2D.Impulse);
        }
    }

    // 🔥 chamado no FINAL da animação
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