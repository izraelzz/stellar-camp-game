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

    [Header("Gravity Feel")]
    public float fallMultiplier = 1.6f;
    public float lowJumpMultiplier = 2.0f;

    [Header("Edge Fall Fix")]
    public float edgeFallVelocity = -2f;

    [Header("Landing Fix")]
    public float groundedGraceTime = 0.05f;

    private float velocityY;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float groundedTimer;

    private int jumpCount;
    private bool isGrounded;
    private bool wasGrounded;

    private bool jumpedThisFrame;

    public bool JustLanded { get; private set; } // ⭐ IMPORTANTE

    private PlayerMovement movement;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleTimers();
        HandleInput();

        groundedTimer -= Time.deltaTime;


        JustLanded = false;
        if (!wasGrounded && isGrounded)
        {
            JustLanded = true;
        }

        // EDGE FALL
        if (wasGrounded && !isGrounded)
        {
            if (!jumpedThisFrame && velocityY <= 0)
            {
                velocityY = edgeFallVelocity;
            }
        }

        wasGrounded = isGrounded;
        jumpedThisFrame = false;
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
            if ((coyoteTimer > 0 || jumpCount < maxJumps) && groundedTimer <= 0)
                Jump();
        }

        if (Input.GetButtonUp("Jump") && velocityY > 0)
            velocityY *= jumpCutMultiplier;
    }

    void ApplyGravity()
    {
        if (movement.IsDashing()) return;

        float gravityStep = gravity * Time.fixedDeltaTime;

        if (velocityY < -0.1f && velocityY > -2f)
            gravityStep *= 0.5f;

        if (velocityY < 0)
        {
            velocityY -= gravityStep * fallMultiplier;
        }
        else if (velocityY > 0 && !Input.GetButton("Jump"))
        {
            velocityY -= gravityStep * lowJumpMultiplier;
        }
        else
        {
            velocityY -= gravityStep;
        }

        if (Mathf.Abs(velocityY) < 0.15f)
            velocityY = 0;

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
        jumpedThisFrame = true;
    }

    public float GetVelocityY() => velocityY;
    public void ResetVerticalVelocity() => velocityY = 0;
    public bool IsGrounded() => isGrounded;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    groundedTimer = groundedGraceTime;

                    velocityY = 0;
                    jumpCount = 0;
                    break;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    return;
                }
            }
        }

        isGrounded = false;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}