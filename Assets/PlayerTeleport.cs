using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleportation Settings")]
    [Tooltip("New position where the player will be teleported when touching the UpDetector trigger.")]
    public Vector2 teleportPosition; // Posición de teletransporte ajustable en el inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UpDetector"))
        {
            transform.position = teleportPosition; // Teletransporta al jugador
        }
    }
}
