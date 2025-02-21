using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockObject;   // Asigna el GameObject "Rock" de la escena
    public Transform spawnPoint;    // Posición donde la roca aparecerá
    public float spawnDelay = 0f;   // Retardo opcional antes de activar la roca

    private bool hasSpawned = false; // Evita múltiples activaciones

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasSpawned && collision.CompareTag("Player"))
        {
            hasSpawned = true;
            Invoke("SpawnRock", spawnDelay);
        }
    }

    private void SpawnRock()
    {
        // Posicionar la roca en el spawnPoint y activarla
        rockObject.transform.position = spawnPoint.position;
        rockObject.SetActive(true);
    }

    // Método para resetear el flag y permitir un nuevo spawn
    public void ResetSpawner()
    {
        hasSpawned = false;
    }
}
