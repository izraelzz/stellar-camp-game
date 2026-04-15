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

    void Awake()
    {
        composer = vcam.GetComponent<CinemachinePositionComposer>();

        if (player == null)
            player = FindFirstObjectByType<PlayerMovement>();

        rb = player.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        DirectionBias();
        VerticalResponse();
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