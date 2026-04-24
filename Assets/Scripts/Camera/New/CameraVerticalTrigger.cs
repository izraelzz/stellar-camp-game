using UnityEngine;
using UnityEngine.Events;

public class CameraVerticalTrigger : MonoBehaviour
{
public string collisionTag = "Player";

[Header("Eventos")]
public UnityEvent onTopSide;    // câmera zona de cima
public UnityEvent onBottomSide; // câmera zona de baixo

private bool isOnTopSide;

private void OnTriggerStay2D(Collider2D collision)
{
    if (!collision.CompareTag(collisionTag))
        return;

    float playerY = collision.transform.position.y;
    float triggerY = transform.position.y;

    bool currentlyTop = playerY > triggerY;

    // Só troca se mudou de lado
    if (currentlyTop != isOnTopSide)
    {
        isOnTopSide = currentlyTop;

        if (currentlyTop)
            onTopSide?.Invoke();
        else
            onBottomSide?.Invoke();
    }
}

}