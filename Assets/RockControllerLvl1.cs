using UnityEngine;
using UnityEngine.SceneManagement; // Si usas esta forma de reinicio

public class RockControllerLvl1 : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float speed = 15f;  // Velocidad de la roca

    private Transform doorTarget;

    void Start()
    {
        // Buscar la puerta con el tag "Wall"
        GameObject doorObject = GameObject.FindGameObjectWithTag("Wall");
        if (doorObject != null)
        {
            doorTarget = doorObject.transform;
        }
        else
        {
            Debug.LogError("No se encontr� la puerta con el tag 'Wall'.");
        }
    }

    void Update()
    {
        if (doorTarget != null)
        {
            // Calcula la direcci�n hacia la puerta
            Vector3 direction = doorTarget.position - transform.position;
            // Forzar que la componente vertical sea cero
            direction.y = 0;
            // Normaliza la direcci�n si no es cero para mantener una velocidad constante
            if (direction != Vector3.zero)
                direction.Normalize();
            // Mueve la roca solo horizontalmente
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisi�n detectada con: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Colisi�n con Wall detectada");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisi�n con Player detectada");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
            gameObject.SetActive(false);
            RockSpawner spawner = FindObjectOfType<RockSpawner>();
            if (spawner != null)
            {
                spawner.ResetSpawner();
            }
        }
    }

}