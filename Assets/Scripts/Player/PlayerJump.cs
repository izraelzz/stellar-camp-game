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

        if (velocityY < 0)
        {
            velocityY -= gravity * fallMultiplier * Time.fixedDeltaTime;
        }
        else if (velocityY > 0 && !Input.GetButton("Jump"))
        {
            velocityY -= gravity * lowJumpMultiplier * Time.fixedDeltaTime;
        }
        else
        {
            velocityY -= gravity * Time.fixedDeltaTime;
        }

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

    public bool IsGrounded()
    {
        return isGrounded;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
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