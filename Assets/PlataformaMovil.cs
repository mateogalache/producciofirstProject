using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tiene la etiqueta "Player"
        {
            other.transform.SetParent(transform); // Hace que el jugador se mueva con la plataforma
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Libera al jugador al salir de la plataforma
        }
    }
}
