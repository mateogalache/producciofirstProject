using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Posici�n actual del checkpoint
    private Vector3 currentCheckpoint = Vector3.zero;

    private void Awake()
    {
        // Implementaci�n del patr�n Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escena
            Debug.Log("GameManager creado y persistente.");
        }
        else
        {
            Debug.LogWarning("GameManager ya existe. Se destruir� el duplicado.");
            Destroy(gameObject);
            return; //
        }
    }

    public bool HasSavedProgress()
    {
        // Inicializar el checkpoint al inicio del nivel
        return currentCheckpoint != Vector3.zero;
    }

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint actualizado a: " + currentCheckpoint);
    }

    // M�todo para reiniciar el nivel desde el checkpoint
    public void RestartLevel()
    {
        // Encuentra al jugador y reposici�nalo
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentCheckpoint;
            // Opcional: Resetea el estado del jugador, como la salud, etc.
        }

        // Opcional: Reiniciar la escena completa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartNewGame()
    {
        currentCheckpoint = Vector3.zero; // Reseteamos el checkpoint al valor inicial
        Debug.Log("Juego reseteado");
        SceneManager.LoadScene("Level1");
    }
}
