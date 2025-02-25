using UnityEngine;

public class SecretDoorTrigger : MonoBehaviour
{
    [Tooltip("Pieza secreta de la pared que se abrirá")]
    public GameObject doorPiece;  // Asigna el GameObject "PiezaSecreta"

    private Collider2D doorCollider;
    private SpriteRenderer doorRenderer;

    void Start()
    {
        if (doorPiece != null)
        {
            doorCollider = doorPiece.GetComponent<Collider2D>();
            doorRenderer = doorPiece.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("No se ha asignado doorPiece en SecretDoorTrigger.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Abre la puerta secreta: deshabilita el collider y cambia la opacidad (opcional)
            if (doorCollider != null)
                doorCollider.enabled = false;
            if (doorRenderer != null)
            {
                Color color = doorRenderer.color;
                color.a = 0f; // Menor opacidad (puedes poner 0 para invisible)
                doorRenderer.color = color;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Restaura el estado: vuelve a habilitar el collider y la opacidad original
            if (doorCollider != null)
                doorCollider.enabled = true;
            if (doorRenderer != null)
            {
                Color color = doorRenderer.color;
                color.a = 1f;
                doorRenderer.color = color;
            }
        }
    }
}
