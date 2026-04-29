using UnityEngine;

public class SlimeHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 3;
    private int currentHealth;

    private SlimeController controller;
    private HitFlashController flash;

    public MobSound mobSound;

    void Awake()
    {
        currentHealth = maxHealth;
        controller = GetComponent<SlimeController>();
        flash = GetComponentInChildren<HitFlashController>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (controller == null || controller.IsDead()) return;

        currentHealth -= damage;

        Debug.Log("Slime tomou dano! Vida: " + currentHealth);

        mobSound?.PlayHit();

        flash?.Flash();

        controller.TakeHit(knockback);

        if (currentHealth <= 0)
        {
            controller.Die();
        }
    }
}