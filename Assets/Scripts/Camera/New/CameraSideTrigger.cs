using UnityEngine;
using UnityEngine.Events;

public class CameraSideTrigger : MonoBehaviour
{
public string collisionTag = "Player";

[Header("Eventos")]
public UnityEvent onLeftSide;  // câmera zona 1
public UnityEvent onRightSide; // câmera zona 2

private bool isOnRightSide;

private void OnTriggerStay2D(Collider2D collision)
{
    if (!collision.CompareTag(collisionTag))
        return;

    float playerX = collision.transform.position.x;
    float triggerX = transform.position.x;

    bool currentlyRight = playerX > triggerX;

    // Só troca se mudou de lado
    if (currentlyRight != isOnRightSide)
    {
        isOnRightSide = currentlyRight;

        if (currentlyRight)
            onRightSide?.Invoke();
        else
            onLeftSide?.Invoke();
    }
}

}