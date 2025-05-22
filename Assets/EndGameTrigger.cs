using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Nombre de la escena a cargar

    private void OnTriggerEnter2D(Collider2D other)
    {
            Debug.Log("Fin del juego. Cargando la escena: " + nextSceneName);
        if (other.CompareTag("EndGame"))
        {
            Debug.Log("Fin del juego. Cargando la escena: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
