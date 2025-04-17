using UnityEngine;
using System.Collections;

public class RockSpawnerLvl2 : MonoBehaviour
{
    public GameObject[] rockObjects;   // Array de rocas
    public Transform spawnPoint;       // Posición donde aparecerán
    public float spawnDelay = 0f;      // Retardo inicial antes de activar
    public float timeBetweenRocks = 1f; // Tiempo entre cada roca
    public float velocidad = 5f;       // Velocidad de movimiento (siempre hacia izquierda)
    
    private bool hasSpawned = false;
    private int currentRockIndex = 0;  // Índice de la roca actual

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
            StartCoroutine(SpawnRocksOneByOne());
        }
    }

    private IEnumerator SpawnRocksOneByOne()
    {
        yield return new WaitForSeconds(spawnDelay); // Espera inicial
        
        while (currentRockIndex < rockObjects.Length)
        {
            // Activar y posicionar la roca actual
            GameObject currentRock = rockObjects[currentRockIndex];
            currentRock.SetActive(true);
            currentRock.transform.position = spawnPoint.position;
            
            // Configurar movimiento hacia izquierda
            Rigidbody2D rb = currentRock.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.left * velocidad;
            }
            else
            {
                Debug.LogError("La roca no tiene Rigidbody2D!", currentRock);
            }

            currentRockIndex++; // Pasar a la siguiente roca
            yield return new WaitForSeconds(timeBetweenRocks); // Espera entre rocas
        }
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
        currentRockIndex = 0;
        
        // Desactivar todas las rocas al resetear
        foreach (GameObject rock in rockObjects)
        {
            rock.SetActive(false);
        }
    }
}