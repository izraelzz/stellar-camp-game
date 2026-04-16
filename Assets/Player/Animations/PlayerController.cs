using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerCombat combat;
    private Animator anim;
    private Rigidbody2D rb;

    private PlayerState currentState;
    private PlayerState lastState;

    private int lastComboStep = 0;

    private bool wasFalling;
    private float fallStartTime;
    private bool wasGrounded;

    // 🔥 LAND CONTROL
    private float landTimer;
    private float landDuration = 0.12f;

    enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Falling,
        Land,
        Attack
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
        bool grounded = movement.IsGroundedRaw();

        // 🔥 CANCEL DE ATAQUE COM PULO
        if (combat.IsAttacking())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (combat.TryCancelAttack())
                {
                    movement.OnJumpInput(); // executa pulo
                }
            }

            // continua atacando se não cancelou
            if (combat.IsAttacking())
            {
                currentState = PlayerState.Attack;
                wasGrounded = grounded;
                return;
            }
        }

        // 🔥 DETECTA LAND
        if (!wasGrounded && grounded)
        {
            currentState = PlayerState.Land;
            landTimer = landDuration;
            wasGrounded = grounded;
            return;
        }

        wasGrounded = grounded;

        // 🔒 SEGURA LAND
        if (currentState == PlayerState.Land)
        {
            landTimer -= Time.deltaTime;

            if (landTimer > 0)
                return;
        }

        float yVel = rb.linearVelocity.y;

        // 🟣 NO AR
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
                currentState = PlayerState.Falling;
                return;
            }

            currentState = PlayerState.Fall;
            return;
        }

        // 🟢 NO CHÃO
        wasFalling = false;

        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) > 0.1f)
            currentState = PlayerState.Run;
        else
            currentState = PlayerState.Idle;
    }

    void PlayAnimation()
    {
        // 🔥 ATAQUE (tratamento especial)
        if (currentState == PlayerState.Attack)
        {
            int combo = combat.GetComboStep();

            if (lastState != PlayerState.Attack || combo != lastComboStep)
            {
                anim.Play("Attack" + combo, 0, 0f);
                lastComboStep = combo;
            }

            lastState = currentState;
            return;
        }

        // 🔥 EVITA REPLAY DESNECESSÁRIO
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

            case PlayerState.Falling:
                anim.Play("Falling");
                break;

            case PlayerState.Land:
                anim.Play("Land", 0, 0f);
                break;
        }
    }
}