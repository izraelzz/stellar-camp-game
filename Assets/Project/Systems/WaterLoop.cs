using UnityEngine;

public class WaterLoop : MonoBehaviour
{
    public float speed = 0.5f;
    public float width = 10f; // largura do tilemap

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x >= startPos.x + width)
        {
            transform.position = startPos;
        }
    }
}