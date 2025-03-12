using UnityEngine;

public class PlayerPlatformStick : MonoBehaviour
{
    private Transform platformTransform; // Referencia a la plataforma
    private Vector3 platformOffset; // Diferencia de posición entre el jugador y la plataforma

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlatformDetector"))
        {
            platformTransform = other.transform;
            platformOffset = transform.position - platformTransform.position;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlatformDetector"))
        {
            platformTransform = null;
        }
    }

    void Update()
    {
        if (platformTransform != null)
        {
            // Si el jugador está en la plataforma, ajusta su posición para que se mueva con la plataforma
            transform.position = platformTransform.position + platformOffset;
        }
    }
}
