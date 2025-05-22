// GameManagerLvl2.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLvl2 : MonoBehaviour
{
    public static GameManagerLvl2 Instance { get; private set; }

    [SerializeField] private Vector3 initialSpawnPosition = new Vector3(-1.920908f, -14.60353f, -1.869639f);


    private Vector3 currentCheckpoint = Vector3.zero;
    private bool hasProgress;
    private string currentLevel;

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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevel = scene.name;

        if (scene.name == "Level2")
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            // Si no hay progreso, colocamos al jugador en la posición inicial
            if (!hasProgress)
            {
                currentCheckpoint = initialSpawnPosition;
                hasProgress = true;
                player.transform.position = currentCheckpoint;

                Debug.Log("[GameManagerLvl2] Spawn inicial en: " + currentCheckpoint);
            }
            else
            {
                player.transform.position = currentCheckpoint;
                Debug.Log("[GameManagerLvl2] Reposicionado en checkpoint: " + currentCheckpoint);
            }
        }
    }


    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        hasProgress = true;
        Debug.Log("[GameManagerLvl2] Checkpoint actualizado a: " + newCheckpoint);
    }

    public void RespawnPlayer(GameObject player)
    {
        if (!hasProgress) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        player.transform.position = currentCheckpoint;
        Debug.Log("[GameManagerLvl2] Jugador respawneado en: " + currentCheckpoint);
    }

    public void RestartLevel()
    {
        if (!hasProgress || string.IsNullOrEmpty(currentLevel))
        {
            Debug.LogWarning("[GameManagerLvl2] No hay checkpoint o nivel cargado. Reiniciando como nuevo.");
            SceneManager.LoadScene("Level2");
            return;
        }

        SceneManager.LoadScene(currentLevel);
    }
}