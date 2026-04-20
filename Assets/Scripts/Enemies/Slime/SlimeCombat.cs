using UnityEngine;
using System.Collections.Generic;

public class SlimeCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1.2f;
    public LayerMask playerLayer;
    public int damage = 1;

    [Header("Knockback")]
    public float knockbackForce = 6f;
    public float knockbackUp = 3f;

    public void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        HashSet<GameObject> hitPlayers = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            if (hitPlayers.Contains(hit.gameObject)) continue;

            PlayerHealth player = hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
                Vector2 knockback = new Vector2(dir * knockbackForce, knockbackUp);

                player.TakeDamage(damage, knockback);

                hitPlayers.Add(hit.gameObject);

                // 🎥 CAMERA SHAKE (mais pesado que antes)
                CameraShake2D.Instance?.Shake(0.12f, 1.5f, 12f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}