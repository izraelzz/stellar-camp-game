using UnityEngine;

public class SlimeHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private SlimeController controller;
    private HitFlashController flash; // 🔥 NOVO

    void Awake()
    {
        currentHealth = maxHealth;
        controller = GetComponent<SlimeController>();
        flash = GetComponentInChildren<HitFlashController>(); // 🔥 importante
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (controller == null || controller.IsDead()) return;

        currentHealth -= damage;

        Debug.Log("Slime tomou dano! Vida: " + currentHealth);

        // 🔥 FLASH AQUI
        flash?.Flash();

        controller.TakeHit(knockback);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        controller.Die();
    }
}