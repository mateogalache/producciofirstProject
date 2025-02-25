using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float jumpForce = 20f;  // Fuerza de impulso vertical

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Reinicia la velocidad vertical para un impulso limpio
                rb.velocity = new Vector2(rb.velocity.x, 0);
                // Aplica la fuerza de impulso
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}
