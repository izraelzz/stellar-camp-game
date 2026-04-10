using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem2D : MonoBehaviour
{
    [Header("Refs")]
    public CinemachineCamera vcam;
    public PlayerMovement player;

    private CinemachinePositionComposer composer;

    [Header("Look Ahead")]
    public float lookAhead = 2.2f;
    public float smooth = 6f;

    private float currentX;

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
        float dir = Mathf.Sign(player.VelocityX);

        if (Mathf.Abs(player.VelocityX) < 0.1f)
            dir = 0;

        float target = dir * lookAhead;

        currentX = Mathf.Lerp(currentX, target, Time.deltaTime * smooth);

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