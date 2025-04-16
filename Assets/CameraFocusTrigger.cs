using System.Collections;
using UnityEngine;

public class CameraFocusTrigger : MonoBehaviour
{
    [Header("Referencia a la c�mara")]
    public AdvancedCameraFollow cameraFollow; // Se puede asignar manualmente o se obtendr� de Camera.main

    [Header("Configuraci�n del �rea de Enfoque")]
    public Vector2 minFocus = new Vector2(450, 1);
    public Vector2 maxFocus = new Vector2(500, 3);

    [Header("Par�metros de Transici�n")]
    public float focusDuration = 2f;      // Tiempo que la c�mara se mantiene en el �rea de enfoque
    public float transitionSpeed = 1f;    // Tiempo que tarda en ir y volver a la posici�n de seguimiento

    private bool activated = false;

    // Referencias al control y f�sica del jugador
    private PlayerMovementLvl2 playerMovement;
    private Rigidbody2D playerRb;
    private RigidbodyConstraints2D originalConstraints;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprueba que el objeto que entra es el jugador
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;
            Debug.Log("Trigger detectado en CameraFocusTrigger");

            // Desactivar el control del jugador
            playerMovement = other.GetComponent<PlayerMovementLvl2>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                Debug.Log("Componente PlayerMovementLvl2 desactivado.");
            }
            else
            {
                Debug.LogWarning("No se encontr� el componente PlayerMovementLvl2 en el jugador.");
            }

            // Congelar el Rigidbody2D para evitar desplazamiento (por ejemplo, por inercia o gravedad)
            playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                originalConstraints = playerRb.constraints;
                // Adem�s, ponemos la velocidad a cero para asegurarnos de que se detenga
                playerRb.velocity = Vector2.zero;
                playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
                Debug.Log("Rigidbody2D del jugador congelado.");
            }
            else
            {
                Debug.LogWarning("No se encontr� Rigidbody2D en el jugador.");
            }

            // Si no se ha asignado la referencia de la c�mara, se intenta obtener desde la c�mara principal
            if (cameraFollow == null)
            {
                cameraFollow = Camera.main.GetComponent<AdvancedCameraFollow>();
                if (cameraFollow == null)
                {
                    Debug.LogWarning("No se encontr� el componente AdvancedCameraFollow en la c�mara principal.");
                    return;
                }
            }

            // Calcular el punto de enfoque (centro entre minFocus y maxFocus)
            Vector3 focusPoint = new Vector3((minFocus.x + maxFocus.x) / 2f,
                                              (minFocus.y + maxFocus.y) / 2f,
                                              cameraFollow.offset.z);
            StartCoroutine(FocusCameraRoutine(focusPoint));
        }
    }

    private IEnumerator FocusCameraRoutine(Vector3 focusPoint)
    {
        // Detener el seguimiento normal de la c�mara
        cameraFollow.SetCameraMovement(false);

        // Transici�n suave hacia el punto de enfoque
        float elapsed = 0f;
        Vector3 startPos = cameraFollow.transform.position;
        while (elapsed < transitionSpeed)
        {
            cameraFollow.transform.position = Vector3.Lerp(startPos, focusPoint, elapsed / transitionSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraFollow.transform.position = focusPoint;

        // Mantener la posici�n de enfoque por el tiempo especificado
        yield return new WaitForSeconds(focusDuration);

        // Transici�n suave de regreso a la posici�n de seguimiento del jugador
        elapsed = 0f;
        startPos = cameraFollow.transform.position;
        Vector3 targetFollowPos = cameraFollow.target.position + cameraFollow.offset;
        while (elapsed < transitionSpeed)
        {
            cameraFollow.transform.position = Vector3.Lerp(startPos, targetFollowPos, elapsed / transitionSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraFollow.transform.position = targetFollowPos;

        // Reanudar el seguimiento de la c�mara
        cameraFollow.SetCameraMovement(true);

        // Restaurar las restricciones originales y reactivar el control del jugador
        if (playerRb != null)
        {
            playerRb.constraints = originalConstraints;
            playerRb.WakeUp(); // Despertar al Rigidbody en caso de que siga en reposo
            Debug.Log("Rigidbody2D del jugador descongelado.");
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("Componente PlayerMovementLvl2 reactivado.");
        }
    }
}
