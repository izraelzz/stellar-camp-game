using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem2D : MonoBehaviour
{
    public CinemachineCamera vcam;
    public PlayerMovement player;

    private Rigidbody2D rb;
    private CinemachinePositionComposer composer;

    [Header("Look Ahead")]
    public float lookAhead = 2.5f;
    public float accel = 8f;
    public float decel = 4f;
    public float maxLook = 3f;

    private float currentX;
    private float lastDir;

    [Header("Vertical")]
    public float upDamping = 1.6f;
    public float downDamping = 0.5f;
    public float groundedDamping = 1.2f;

    private float currentYDamping;

    [Header("Look Up/Down")]
    public float lookOffsetAmount = 2f;
    public float lookSmoothSpeed = 5f;
    private float currentLookOffsetY;
    private float targetLookOffsetY;

    void Awake()
    {
        composer = vcam.GetComponent<CinemachinePositionComposer>();

        if (player == null)
            player = FindFirstObjectByType<PlayerMovement>();

        rb = player.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        HandleLookUpDown();
        DirectionBias();
        VerticalResponse();
    }

    void HandleLookUpDown()
    {
        // Check if player is idle (not moving, not jumping, not dashing, not wall jumping)
        bool isIdle = IsPlayerIdle();

        if (isIdle)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (verticalInput > 0)
            {
                // W pressed - look up
                targetLookOffsetY = lookOffsetAmount;
            }
            else if (verticalInput < 0)
            {
                // S pressed - look down
                targetLookOffsetY = -lookOffsetAmount;
            }
            else
            {
                // No vertical input - return to center
                targetLookOffsetY = 0;
            }
        }
        else
        {
            // Not idle - reset to center
            targetLookOffsetY = 0;
        }

        // Smoothly interpolate the offset
        currentLookOffsetY = Mathf.Lerp(currentLookOffsetY, targetLookOffsetY, Time.deltaTime * lookSmoothSpeed);

        // Apply to camera
        Vector3 offset = composer.TargetOffset;
        offset.y = currentLookOffsetY;
        composer.TargetOffset = offset;
    }

    bool IsPlayerIdle()
    {
        // Check if player is not moving horizontally
        bool notMoving = Mathf.Abs(rb.linearVelocity.x) < 0.1f;
        
        // Check if player is not in any action state
        bool notJumping = !player.IsJumping;
        bool notDashing = !player.IsDashing;
        bool notWallJumping = !player.IsWallJumping;
        
        return notMoving && notJumping && notDashing && notWallJumping;
    }

    void DirectionBias()
    {
        float vx = rb.linearVelocity.x;
        float input = Mathf.Sign(vx);

        if (Mathf.Abs(vx) > 0.1f)
            lastDir = input;

        float target = lastDir * lookAhead;
        float speed = (Mathf.Abs(vx) > 0.1f) ? accel : decel;

        currentX = Mathf.Lerp(currentX, target, Time.deltaTime * speed);
        currentX = Mathf.Clamp(currentX, -maxLook, maxLook);

        Vector3 offset = composer.TargetOffset;
        offset.x = currentX;
        composer.TargetOffset = offset;
    }

    void VerticalResponse()
    {
        float vy = rb.linearVelocity.y;
        bool grounded = player.LastOnGroundTime > 0;

        float targetDamping = grounded
            ? groundedDamping
            : (vy > 0 ? upDamping : downDamping);

        currentYDamping = Mathf.Lerp(currentYDamping, targetDamping, Time.deltaTime * 5f);

        Vector3 damping = composer.Damping;
        damping.y = currentYDamping;
        composer.Damping = damping;
    }
}