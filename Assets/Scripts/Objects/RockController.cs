using UnityEngine;
using UnityEngine.SceneManagement; // Si usas esta forma de reinicio

public class RockController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float speed = 15f;  // Velocidad de la roca

    private Transform doorTarget;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión con Player detectada");
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
