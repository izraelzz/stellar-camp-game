using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem2D : MonoBehaviour
{
    [Header("Refs")]
    public CinemachineCamera vcam;
    public PlayerMovement player;

    private CinemachinePositionComposer composer;

    [Header("Look Ahead")]
    public float lookAhead = 2.5f;

    // Separação importante
    public float accel = 8f;     // quando começa a andar
    public float decel = 4f;     // quando para

    public float maxLook = 3f;

    private float currentX;
    private float lastDir;

    [Header("Vertical")]
    public float upDamping = 1.6f;
    public float downDamping = 0.5f;
    public float groundedDamping = 1.2f;

    void Awake()
    {
        composer = vcam.GetComponent<CinemachinePositionComposer>();
    }

    void LateUpdate()
    {
        DirectionBias();
        VerticalResponse();
    }

    void DirectionBias()
    {
        float input = Mathf.Sign(player.VelocityX);

        // guarda última direção válida
        if (Mathf.Abs(player.VelocityX) > 0.1f)
            lastDir = input;

        float target = lastDir * lookAhead;

        // desaceleração diferente quando para
        float speed = (Mathf.Abs(player.VelocityX) > 0.1f) ? accel : decel;

        currentX = Mathf.Lerp(currentX, target, Time.deltaTime * speed);

        // limita pra não exagerar (ESSENCIAL pro feeling Silksong)
        currentX = Mathf.Clamp(currentX, -maxLook, maxLook);

        Vector3 offset = composer.TargetOffset;
        offset.x = currentX;
        composer.TargetOffset = offset;
    }

    void VerticalResponse()
    {
        float vy = player.VelocityY;

        if (!player.IsGrounded)
        {
            composer.Damping.y = (vy > 0) ? upDamping : downDamping;
        }
        else
        {
            composer.Damping.y = groundedDamping;
        }
    }
}