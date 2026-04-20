using UnityEngine;

public class BeeHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 2;
    private int currentHealth;

    private BeeController controller;
    private HitFlashController flash;

    void Awake()
    {
        currentHealth = maxHealth;
        controller = GetComponent<BeeController>();

        flash = GetComponentInChildren<HitFlashController>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (controller == null || controller.IsDead()) return;

        currentHealth -= damage;

        Debug.Log("Bee tomou dano! Vida: " + currentHealth);

        flash?.Flash();

        controller.TakeHit(knockback);

        if (currentHealth <= 0)
        {
            controller.Die();
        }
    }
}