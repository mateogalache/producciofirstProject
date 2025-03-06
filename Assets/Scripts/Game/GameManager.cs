using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Posici�n actual del checkpoint
    private Vector3 currentCheckpoint;

    private void Awake()
    {
        // Implementaci�n del patr�n Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Debug.LogWarning("GameManager ya existe. Se destruir� el duplicado.");
            Destroy(gameObject);
            return; //
        }
    }

    private void Start()
    {
        // Inicializar el checkpoint al inicio del nivel
        currentCheckpoint = Vector3.zero; // Puedes ajustar esto seg�n tu nivel
    }

    // M�todo para actualizar el checkpoint
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
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
