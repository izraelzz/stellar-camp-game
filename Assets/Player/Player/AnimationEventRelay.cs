using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerCombat combat;

    void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
    }

    public void PerformAttack()
    {
        combat.PerformAttack();
    }

    public void EndAttack()
    {
        combat.EndAttack();
    }
}