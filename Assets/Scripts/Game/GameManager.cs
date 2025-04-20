using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Posición actual del checkpoint
    private Vector3 currentCheckpoint = Vector3.zero;
    private bool hasProgress;
    private string lastLevelName;

    private void Awake()
    {
        /*
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escena
            Debug.Log("GameManager creado y persistente.");
        }
        else
        {
            Debug.LogWarning("GameManager ya existe. Se destruirá el duplicado.");
            Destroy(gameObject);
            return; //
        }*/

        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializamos el checkpoint cuando cargue una escena de nivel
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level"))
        {
            /*
            // Al entrar al nivel por primera vez, tomamos la posición inicial del jugador
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && !hasProgress)
            {
                currentCheckpoint = player.transform.position;
                hasProgress = true;
                Debug.Log("Checkpoint inicial establecido en " + currentCheckpoint);
            }*/

            lastLevelName = scene.name;

            // Encuentra jugador
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            if (!hasProgress)
            {
                
                  currentCheckpoint = player.transform.position;
                  hasProgress = true;
                  Debug.Log("Checkpoint inicial en " + currentCheckpoint);
                
            } else
            {
                player.transform.position = currentCheckpoint;
                Debug.Log($"[GameManager] Reposicionado en checkpoint {currentCheckpoint}");
            }
        }
    }

    public bool HasSavedProgress()
    {
        // Inicializar el checkpoint al inicio del nivel
        //return currentCheckpoint != Vector3.zero;
        return hasProgress;
    }

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        hasProgress = true;
        Debug.Log("Checkpoint actualizado a: " + currentCheckpoint);
    }

    // Método para reiniciar el nivel desde el checkpoint
    public void RestartLevel()
    {
        if (string.IsNullOrEmpty(lastLevelName) || !hasProgress)
        {
            Debug.LogWarning("[GameManager] No hay nivel previo o progreso: iniciando nueva partida.");
            StartNewGame();
            return;
        }
        SceneManager.LoadScene(lastLevelName);
        /*
        // Encuentra al jugador y reposiciónalo
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentCheckpoint;
            // Opcional: Resetea el estado del jugador, como la salud, etc.
        }

        // Opcional: Reiniciar la escena completa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        */
    }

    public void StartNewGame()
    {
        hasProgress = false;
        currentCheckpoint = Vector3.zero; // Reseteamos el checkpoint al valor inicial
        lastLevelName = "Level1";
        Debug.Log("Juego reseteado");
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }
}
