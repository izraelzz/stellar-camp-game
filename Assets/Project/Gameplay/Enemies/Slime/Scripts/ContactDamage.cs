using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [Header("Damage")]
    public int contactDamage = 1;
    public float contactCooldown = 0.5f;

    [Header("Knockback")]
    public float contactKnockback = 5f;
    public float contactKnockbackUp = 3f;
    public float contactKnockbackSide = 5f;
    // threshold in world units to consider the contact as "from above"
    public float topContactThreshold = 0.2f;

    private float lastContactTime = -999f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsDead) return;

        if (Time.time < lastContactTime + contactCooldown)
            return;

        // approximate whether the player hit the mob from above using Y difference
        float yDiff = other.transform.position.y - transform.position.y;

        Vector2 knockback;

        if (yDiff > topContactThreshold)
        {
            float side = Mathf.Sign(other.transform.position.x - transform.position.x);
            if (side == 0)
                side = Random.value < 0.5f ? -1 : 1;

            knockback = new Vector2(side * contactKnockbackSide, contactKnockbackUp);
        }
        else
        {
            float dir = Mathf.Sign(other.transform.position.x - transform.position.x);
            if (dir == 0)
                dir = Random.value < 0.5f ? -1 : 1;

            knockback = new Vector2(dir * contactKnockback, contactKnockbackUp);
        }

        playerHealth.TakeDamage(contactDamage, knockback);
        lastContactTime = Time.time;
    }
}
