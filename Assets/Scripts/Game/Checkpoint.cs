using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Opcional: Puedes añadir una variable para marcar si el checkpoint ya ha sido activado
    private bool isActivated = false;

    public TutorialUI tutorialUI;

    private void Start()
    {
        if (tutorialUI == null)
        {
            tutorialUI = FindObjectOfType<TutorialUI>(); // Busca automáticamente si no está asignado
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            // Actualizar el checkpoint en el GameManager
            if (GameManager.Instance != null)
            {
                // Ajusta la posición según tu diseño (por ejemplo, ligeramente por encima del checkpoint)
                Vector3 checkpointPosition = transform.position;
                GameManager.Instance.UpdateCheckpoint(checkpointPosition);
                isActivated = true;

                // Opcional: Añadir feedback visual o auditivo aquí
                ActivateCheckpointVisual();

                if (tutorialUI != null)
                {
                    tutorialUI.ShowMessage("¡Checkpoint alcanzado!");
                } else
                {
                    Debug.LogError("TutorialUI no está asignado en el Inspector.");
                }

                   
            }
            else
            {
                Debug.LogError("GameManager no está presente en la escena.");
            }
        }
    }

    private void ActivateCheckpointVisual()
    {
        // Implementa una señal visual de que el checkpoint ha sido activado
        // Por ejemplo, cambiar el color del checkpoint, activar una luz, etc.
        // Aquí hay un ejemplo simple cambiando el color del SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.green; // Cambia el color a verde al activarse
        }

        // Puedes añadir partículas, sonidos, etc.
    }

}
