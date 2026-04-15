using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerMovement movement;

    private bool wasGrounded;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        movement = GetComponentInParent<PlayerMovement>();
    }

void Update()
{
    float velocityY = rb.linearVelocity.y;

    bool grounded = movement.IsGroundedRaw();

    // 🔥 FORÇA AIRBORNE SE ESTIVER PULANDO
    if (movement.IsJumping || movement.IsWallJumping)
        grounded = false;

    anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

    float animVelocityY = grounded ? 0 : velocityY;
    anim.SetFloat("VelocityY", animVelocityY);

    anim.SetBool("isGrounded", grounded);

    // LAND
    if (!wasGrounded && grounded)
    {
        anim.ResetTrigger("Land");
        anim.SetTrigger("Land");
    }

    wasGrounded = grounded;

    AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

    if (state.IsName("playerLand") && !grounded)
    {
        anim.Play("Airborne");
    }
}
    // ===== ATAQUE =====

    public void PlayAttack(int step)
    {
        anim.SetBool("isAttacking", true);
        anim.SetInteger("ComboStep", step);
    }

    public void ResetAttack()
    {
        anim.SetBool("isAttacking", false);
        anim.SetInteger("ComboStep", 0);
    }

    public void PerformAttack()
    {
        GetComponentInParent<PlayerCombat>().PerformAttack();
    }

    public void EndAttack()
    {
        GetComponentInParent<PlayerCombat>().EndAttack();
    }
}