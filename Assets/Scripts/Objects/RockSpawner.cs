using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject[] rockObject;   // Asigna el GameObject "Rock" de la escena
    public Transform spawnPoint;    // Posici�n donde la roca aparecer�
    public float spawnDelay = 0f;   // Retardo opcional antes de activar la roca

    private bool hasSpawned = false; // Evita m�ltiples activaciones


    private void Start()
    {
        foreach (GameObject rock in rockObject)
        {
            rock.SetActive(false);
        }
    }
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
        foreach(GameObject rock in rockObject)
        {
            rock.SetActive(true);
        }
    }

    // M�todo para resetear el flag y permitir un nuevo spawn
    public void ResetSpawner()
    {
        hasSpawned = false;
    }
}
