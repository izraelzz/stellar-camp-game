using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    #region COMPONENTS

    private Animator anim;
    private Enemy enemy;

    #endregion

    #region STATE CACHE

    private Enemy.State lastState;

    #endregion

    #region UNITY

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemy = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        PlayAnimation();
    }

    #endregion

    #region ANIMATION LOGIC

    void PlayAnimation()
    {
        var state = enemy != null ? enemy.GetCurrentState() : Enemy.State.Idle;

        
        if (state == Enemy.State.Death)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                anim.Play("Death", 0, 0f);

            return;
        }

        
        if (state == lastState) return;
        lastState = state;

        switch (state)
        {
            case Enemy.State.Idle:
                anim.Play("Idle");
                break;

            case Enemy.State.Patrol:
                anim.Play("Walk");
                break;

            case Enemy.State.Chase:
                anim.Play("Run");
                break;

            case Enemy.State.Attack:
                anim.Play("Attack");
                break;

            case Enemy.State.Hit:
                anim.Play("Hit");
                break;
        }
    }

    #endregion
}