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
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VelocityY", rb.linearVelocity.y);
        anim.SetBool("isGrounded", jump.IsGrounded());
    }

    // 🔥 inicia ataque
    public void PlayAttack(int step)
    {
        anim.SetBool("isAttacking", true);
        anim.SetInteger("ComboStep", step);
    }

    // 🔥 finaliza ataque
    public void ResetAttack()
    {
        anim.SetBool("isAttacking", false);
        anim.SetInteger("ComboStep", 0);
    }

    // 🔥 evento no meio da animação
    public void PerformAttack()
    {
        GetComponentInParent<PlayerCombat>().PerformAttack();
    }

    // 🔥 evento no final da animação
    public void EndAttack()
    {
        GetComponentInParent<PlayerCombat>().EndAttack();
    }
}