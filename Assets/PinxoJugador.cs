using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinxoJugador : MonoBehaviour
{

    private Transform playerTransform;
    private Vector3 ultimoCheckpoint;
    private bool debeEsperarJugador = false;

    private void Start()
    {
        ultimoCheckpoint = playerTransform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ManejarColisionJugador();
        }
        else if (other.CompareTag("Checkpoint"))
        {
            ActualizarCheckpoint(other.transform);
        }
    }

    private void ManejarColisionJugador()
    {
        if (playerTransform != null)
        {
            playerTransform.position = ultimoCheckpoint;
            debeEsperarJugador = true;

            // Opcional: Efectos visuales/sonido
            // Ejemplo: PlaySound(), ParticleSystem.Play(), etc.
        }
    }

    private void ActualizarCheckpoint(Transform checkpoint)
    {
        ultimoCheckpoint = checkpoint.position;
        checkpoint.gameObject.SetActive(false);

        // Opcional: Notificar al GameManager
        // GameManager.Instance?.CheckpointAlcanzado(checkpoint.position);
    }
}
