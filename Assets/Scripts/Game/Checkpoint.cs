using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Opcional: Puedes a�adir una variable para marcar si el checkpoint ya ha sido activado
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            // Actualizar el checkpoint en el GameManager
            if (GameManager.Instance != null)
            {
                // Ajusta la posici�n seg�n tu dise�o (por ejemplo, ligeramente por encima del checkpoint)
                Vector3 checkpointPosition = transform.position;
                GameManager.Instance.UpdateCheckpoint(checkpointPosition);
                isActivated = true;

                // Opcional: A�adir feedback visual o auditivo aqu�
                ActivateCheckpointVisual();
            }
            else
            {
                Debug.LogError("GameManager no est� presente en la escena.");
            }
        }
    }

    private void ActivateCheckpointVisual()
    {
        // Implementa una se�al visual de que el checkpoint ha sido activado
        // Por ejemplo, cambiar el color del checkpoint, activar una luz, etc.
        // Aqu� hay un ejemplo simple cambiando el color del SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.green; // Cambia el color a verde al activarse
        }

        // Puedes a�adir part�culas, sonidos, etc.
    }
}
