using UnityEngine;

public class AnimationEventRelayEnemy : MonoBehaviour
{
    private Enemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void PerformAttack()
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy não encontrado no parent do AnimationEventRelayEnemy");
            return;
        }

        enemy.Animation_PerformAttack(); // ou enemy.combat.Attack() dependendo da tua arquitetura
    }

    public void EndAttack()
    {
        if (enemy == null) return;

        enemy.EndAttack();
    }
}