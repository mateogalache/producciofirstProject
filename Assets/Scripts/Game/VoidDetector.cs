using UnityEngine;

public class VoidDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Opcional: Añadir efectos visuales o auditivos aquí

            // Llamar al GameManager para reiniciar el nivel
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
            else
            {
                Debug.LogError("GameManager no está presente en la escena.");
            }
        }

        if (other.CompareTag("Box"))
        {
            // Llamamos al método ResetPosition del script BoxController
            BoxController box = other.GetComponent<BoxController>();
            if (box != null)
            {
                box.ResetPosition();
            }
            else
            {
                Debug.LogError("El objeto con tag 'Box' no tiene el script BoxController asignado.");
            }
        }
    }
}
