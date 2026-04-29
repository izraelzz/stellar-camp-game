using UnityEngine;

public class BeeCombat : MonoBehaviour
{
    [Header("Dano de Contato")]
    public int contactDamage = 1;
    public float contactKnockback = 5f;
    public float contactCooldown = 0.5f;

    [Header("Dano no Dash")]
    public int dashDamage = 1;
    public float dashKnockback = 8f;

    private float lastContactTime;

    private BeeController controller;

    void Awake()
    {
        controller = GetComponent<BeeController>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player == null || player.IsDead) return;

      
        if (controller.IsDashing())
        {
            ApplyDamage(player, dashDamage, dashKnockback);
            return;
        }

     
        if (Time.time < lastContactTime + contactCooldown)
            return;

        ApplyDamage(player, contactDamage, contactKnockback);

        lastContactTime = Time.time;
    }

    void ApplyDamage(PlayerHealth player, int damage, float force)
    {
        float dir = Mathf.Sign(player.transform.position.x - transform.position.x);

        Vector2 knockback = new Vector2(dir * force, 2f);

        player.TakeDamage(damage, knockback);
    }
}