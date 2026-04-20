using UnityEngine;

public class SlimeAnimationController : MonoBehaviour
{
    private SlimeController slime;
    private Animator anim;

    private SlimeController.SlimeState lastState;

    void Awake()
    {
        slime = GetComponentInParent<SlimeController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        PlayAnimation();
    }

    void PlayAnimation()
    {
        var state = slime.GetState();

        // 🔥 DEATH TEM PRIORIDADE ABSOLUTA
        if (state == SlimeController.SlimeState.Death)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                anim.Play("Death", 0, 0f);

            return;
        }

        if (state == lastState) return;
        lastState = state;

        switch (state)
        {
            case SlimeController.SlimeState.Idle:
                anim.Play("Idle");
                break;

            case SlimeController.SlimeState.Patrol:
            case SlimeController.SlimeState.Chase:
                anim.Play("Chase");
                break;

            case SlimeController.SlimeState.Attack:
                anim.Play("Attack");
                break;

            case SlimeController.SlimeState.Hit:
                anim.Play("Hit");
                break;
        }
    }
}