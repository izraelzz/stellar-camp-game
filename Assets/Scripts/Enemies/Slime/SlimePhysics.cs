using UnityEngine;

public class SlimePhysics : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Gravity")]
    public float gravityScale = 4f;
    public float fallMultiplier = 1.5f;
    public float maxFallSpeed = -18f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();
        ApplyGravity();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundSize,
            0,
            groundLayer
        );
    }

    void ApplyGravity()
    {
        if (isGrounded)
        {
            rb.gravityScale = gravityScale;
            return;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallMultiplier;

            // limite de velocidade
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y, maxFallSpeed)
            );
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
    }
}