using UnityEngine;

public class AnimationEventRelayEnemy1 : MonoBehaviour
{
    private SlimeCombat combat;
    private SlimeController controller;

    void Awake()
    {
        combat = GetComponentInParent<SlimeCombat>();
        controller = GetComponentInParent<SlimeController>();
    }

    public void PerformAttack()
    {
        combat.PerformAttack();
    }

    public void EndAttack()
    {
        controller.EndAttack();
    }
}