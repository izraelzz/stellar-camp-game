using UnityEngine;

public class BeeAnimationController : MonoBehaviour
{
    private Animator anim;
    private BeeController controller;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponentInParent<BeeController>();
    }

    void Update()
    {
        if (controller == null) return;

        switch (controller.GetState())
        {
            case BeeController.BeeState.Idle:
            case BeeController.BeeState.Chase:
                PlayIfNot("Fly");
                break;

            case BeeController.BeeState.Attack:


                if (controller.IsDashing())
                {
                    PlayIfNot("Attack");
                }

                else if (controller.IsWindingUp())
                {
                    PlayIfNot("Fly"); 
                }
                else if (controller.IsRecovering())
                {
                    PlayIfNot("Fly");
                }
                else
                {
                    PlayIfNot("Fly");
                }

                break;

            case BeeController.BeeState.Hit:
                PlayIfNot("Hit");
                break;

            case BeeController.BeeState.Death:
                PlayIfNot("Death");
                break;
        }
    }

    void PlayIfNot(string stateName)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            anim.Play(stateName);
        }
    }
}