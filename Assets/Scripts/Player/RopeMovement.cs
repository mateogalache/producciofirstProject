using UnityEngine;

public class RopeMovement : MonoBehaviour
{
    private float vertical;
    private float speed = 8f;
    private bool isRope;
    private bool isClimbing;

    [SerializeField] private Rigidbody2D rb;

    // Referencia a la cámara
    private AdvancedCameraFollow cameraFollow;

    void Start()
    {
        // Busca automáticamente el script de la cámara en la escena
        cameraFollow = Camera.main.GetComponent<AdvancedCameraFollow>();
    }

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        if (isRope && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        else
        {
            rb.gravityScale = 4f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            isRope = true;

            // Si hay una referencia a la cámara, detenemos su movimiento
            if (cameraFollow != null)
            {
                cameraFollow.SetCameraMovement(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            isRope = false;
            isClimbing = false;

            // Cuando el jugador sale de la cuerda, reanudamos el movimiento de la cámara
            if (cameraFollow != null)
            {
                cameraFollow.SetCameraMovement(true);
            }
        }
    }
}
