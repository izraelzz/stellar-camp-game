using UnityEngine;

public class CampHeal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null || ph.IsDead) return;

        ph.HealToMax();

        CheckpointManager.Instance.SetCheckpoint(transform.position);
    }
}