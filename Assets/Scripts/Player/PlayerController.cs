using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerCombat combat;
    private Animator anim;
    private Rigidbody2D rb;

    private PlayerState currentState;
    private PlayerState lastState;

    private bool wasFalling;
    private float fallStartTime;

    enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        MidAir,
        Attack,
        Dash
    }

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        DecideState();
        PlayAnimation();
    }

    void DecideState()
    {
        bool grounded = movement.LastOnGroundTime > 0;

        // 🔥 ATAQUE PRIORIDADE
        if (combat.IsAttacking())
        {
            currentState = PlayerState.Attack;
            return;
        }

        if (movement.IsDashing)
        {
            currentState = PlayerState.Dash;
            return;
        }

        float yVel = rb.linearVelocity.y;

        if (!grounded)
        {
            if (yVel > 0)
            {
                currentState = PlayerState.Jump;
                wasFalling = false;
                return;
            }

            if (!wasFalling)
            {
                currentState = PlayerState.Fall;
                wasFalling = true;
                fallStartTime = Time.time;
                return;
            }

            if (Time.time - fallStartTime > 0.15f)
            {
                currentState = PlayerState.MidAir;
                return;
            }

            currentState = PlayerState.Fall;
            return;
        }

        wasFalling = false;

        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) > 0.1f)
            currentState = PlayerState.Run;
        else
            currentState = PlayerState.Idle;
    }

    void PlayAnimation()
    {
        // 🔥 ATAQUE TRAVADO
        if (currentState == PlayerState.Attack)
        {
            string attackAnim = combat.GetAttackName();

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

            if (!state.IsName(attackAnim))
            {
                anim.Play(attackAnim, 0, 0f);
            }

            lastState = currentState;
            return;
        }

        if (currentState == lastState) return;

        lastState = currentState;

        switch (currentState)
        {
            case PlayerState.Idle:
                anim.Play("Idle");
                break;

            case PlayerState.Run:
                anim.Play("Run");
                break;

            case PlayerState.Jump:
                anim.Play("Jump");
                break;

            case PlayerState.Fall:
                anim.Play("Fall");
                break;

            case PlayerState.MidAir:
                anim.Play("MidAir");
                break;

            case PlayerState.Dash:
                anim.Play("Dash");
                break;
        }
    }
}