using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 80f;

    [Header("Air Control")]
    public float airControlMultiplier = 0.5f;

    [Header("Turn")]
    public float turnSlowMultiplier = 0.6f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.4f;

    [Header("Dash Feel")]
    public float dashEndSlow = 0.5f;

    [Header("Ataque")]
    public Transform attackPoint;

    private Rigidbody2D rb;
    private float velocityX;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private int dashDirection;

    private PlayerJump jump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<PlayerJump>();
    }

    void Update()
    {
        HandleTimers();
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void HandleTimers()
    {
        dashTimer -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;

        if (isDashing && dashTimer <= 0)
        {
            isDashing = false;
            velocityX *= dashEndSlow;
        }
    }

    void HandleInput()
    {
        float input = Input.GetAxisRaw("Horizontal");

        // virar personagem
        if (input > 0)
        {
            transform.localScale = Vector3.one;
            FlipAttackPoint(true);
        }
        else if (input < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            FlipAttackPoint(false);
        }

        float targetSpeed = input * moveSpeed;

        bool grounded = jump.IsGrounded();
        float control = grounded ? 1f : airControlMultiplier;

        float accel = (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration) * control;

        // 🔥 suaviza troca de direção
        if (Mathf.Sign(velocityX) != Mathf.Sign(targetSpeed) && Mathf.Abs(velocityX) > 0.1f)
        {
            velocityX *= turnSlowMultiplier;
        }

        velocityX = Mathf.MoveTowards(
            velocityX,
            targetSpeed,
            accel * Time.deltaTime
        );

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0)
        {
            StartDash(input);
        }
    }

    void ApplyMovement()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(
                dashDirection * dashSpeed,
                rb.linearVelocity.y * 0.2f
            );
            return;
        }

        rb.linearVelocity = new Vector2(velocityX, jump.GetVelocityY());
    }

    void StartDash(float input)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashDirection = input != 0
            ? (int)Mathf.Sign(input)
            : (transform.localScale.x > 0 ? 1 : -1);

        jump.ResetVerticalVelocity();
    }

    void FlipAttackPoint(bool facingRight)
    {
        if (attackPoint == null) return;

        Vector3 pos = attackPoint.localPosition;
        pos.x = Mathf.Abs(pos.x) * (facingRight ? 1 : -1);
        attackPoint.localPosition = pos;
    }

    // 🔥 usado pela câmera
    public bool IsDashing() => isDashing;
    public float VelocityX => rb.linearVelocity.x;
    public float VelocityY => rb.linearVelocity.y;
    public bool IsGrounded => jump.IsGrounded();
}