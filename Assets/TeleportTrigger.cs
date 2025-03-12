using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Posición de Teletransporte")]
    [Tooltip("Asignar el objeto (Transform) a donde se teletransportará el jugador")]
    public Transform teleportTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica que el objeto que entra en el trigger sea el jugador
        if (collision.CompareTag("Player"))
        {
           Debug.Log("TeleportTrigger: Teletransportado a ");
            collision.transform.position = teleportTarget.position;
        }
    }
}