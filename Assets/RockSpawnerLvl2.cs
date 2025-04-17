using UnityEngine;

public class RockSpawnerLvl2 : MonoBehaviour
{
    public GameObject[] rockObjects;   // Array de rocas
    public Transform spawnPoint;       // Posición donde aparecerán
    public float spawnDelay = 0f;      // Retardo antes de activar
    public float velocidad = 5f;       // Velocidad de movimiento (siempre hacia izquierda)
    
    private bool hasSpawned = false;

    private void Start()
    {
        // Desactivar todas las rocas al inicio
        foreach (GameObject rock in rockObjects)
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
        foreach (GameObject rock in rockObjects)
        {
            // Activar y posicionar la roca
            rock.SetActive(true);
            rock.transform.position = spawnPoint.position;
            
            // Configurar movimiento hacia izquierda
            Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.left * velocidad;
            }
            else
            {
                Debug.LogError("La roca no tiene Rigidbody2D!", rock);
            }
        }
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
    }
}