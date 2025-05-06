using UnityEngine;

public class MoveToTrigger : MonoBehaviour
{
    public float speed = 5f;
    public bool moveRight = true;

    private bool isMoving = true;

    void Update()
    {
        if (isMoving)
        {
            Vector2 direction = moveRight ? Vector2.right : Vector2.left;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mirror"))
        {
            isMoving = false;
            Debug.Log("Trigger detectado. Movimiento detenido.");
        }
    }
}
