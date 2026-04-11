using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerJump jump;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        jump = GetComponentInParent<PlayerJump>();
    }

    void Update()
    {
        float velocityY = rb.linearVelocity.y;

        // parâmetros base
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VelocityY", velocityY);
        anim.SetBool("isGrounded", jump.IsGrounded());

       
        if (jump.JustLanded && jump.IsGrounded())
        {
            anim.ResetTrigger("Land"); // limpa possíveis restos
            anim.SetTrigger("Land");
        }

        if (!jump.IsGrounded())
        {
            anim.ResetTrigger("Land");
        }

        
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("playerLand") && !jump.IsGrounded())
        {
            anim.Play("Airborne"); // nome do seu blend tree de jump/fall
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