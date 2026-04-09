using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Pulo")]
    public float jumpForce = 14f;
    public float gravity = 45f;
    public float maxFallSpeed = -25f;

    [Header("Melhorias")]
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.1f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Pulo Duplo")]
    public int maxJumps = 2;
    public float secondJumpMultiplier = 0.7f;

    private float velocityY;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private int jumpCount;
    private bool isGrounded;

    private PlayerMovement movement;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleTimers();
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    void HandleTimers()
    {
        coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;

        if (isGrounded)
            coyoteTimer = coyoteTime;
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBuffer;

        if (jumpBufferTimer > 0)
        {
            if (coyoteTimer > 0 || jumpCount < maxJumps)
                Jump();
        }

        if (Input.GetButtonUp("Jump") && velocityY > 0)
            velocityY *= jumpCutMultiplier;
    }

    void ApplyGravity()
    {
        if (movement.IsDashing()) return;

        velocityY -= gravity * Time.fixedDeltaTime;

        if (velocityY < maxFallSpeed)
            velocityY = maxFallSpeed;
    }

    void Jump()
    {
        jumpCount++;

        velocityY = (jumpCount == 1)
            ? jumpForce
            : jumpForce * secondJumpMultiplier;

        jumpBufferTimer = 0;
    }

    public float GetVelocityY()
    {
        return velocityY;
    }

    public void ResetVerticalVelocity()
    {
        velocityY = 0;
    }

    // 🔥 IMPORTANTE (isso resolve seu erro)
    public bool IsGrounded()
    {
        return isGrounded;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            velocityY = 0;
            jumpCount = 0;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}