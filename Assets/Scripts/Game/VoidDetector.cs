using UnityEngine;

public class VoidDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Opcional: A�adir efectos visuales o auditivos aqu�

            // Llamar al GameManager para reiniciar el nivel
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
            else
            {
                Debug.LogError("GameManager no est� presente en la escena.");
            }
        }
    }
}
