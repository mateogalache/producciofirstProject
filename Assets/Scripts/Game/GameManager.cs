using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Vector3 currentCheckpoint = Vector3.zero;
    private bool hasProgress;
    private string lastLevelName;
    private PlayerDash playerDash;

    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level"))
        {
           
            lastLevelName = scene.name;
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
        return hasProgress;
    }

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        hasProgress = true;
        Debug.Log("Checkpoint actualizado a: " + currentCheckpoint);
    }

    public void RestartLevel()
    {
        if (string.IsNullOrEmpty(lastLevelName) || !hasProgress)
        {
            Debug.LogWarning("[GameManager] No hay nivel previo o progreso: iniciando nueva partida.");
            StartNewGame();
            return;
        }
        SceneManager.LoadScene(lastLevelName);
        
    }

    public void StartNewGame()
    {
        hasProgress = false;
        currentCheckpoint = Vector3.zero;
        lastLevelName = "Level1";
        Debug.Log("Juego reseteado");
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

}
