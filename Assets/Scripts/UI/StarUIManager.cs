using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StarUIManager : MonoBehaviour
{
    // Singleton Instance
    public static StarUIManager Instance;

    [Header("UI Elements")]
    [Tooltip("Referencia al contenedor de estrellas en la UI (GridLayoutGroup)")]
    public Transform starContainer;

    [Tooltip("Prefab de la estrella para la UI")]
    public GameObject starUIPrefab;

    [Header("Audio")]
    [Tooltip("Clip de audio que se reproducir� al recolectar una estrella")]
    public AudioClip collectSound;

    private AudioSource audioSource;
    private int starCount = 0;
    private int maxStars = 14; // N�mero m�ximo de estrellas para terminar el juego

    void Awake()
    {
        // Implementar el patr�n Singleton
        if (Instance == null)
        {
            Instance = this;
            // Opcional: Mantener este objeto entre escenas
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Obtener o a�adir un AudioSource para reproducir sonidos
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Verificar asignaciones
        if (starContainer == null)
            Debug.LogWarning("Star Container no est� asignado en StarUIManager.");

        if (starUIPrefab == null)
            Debug.LogWarning("Star UI Prefab no est� asignado en StarUIManager.");
    }

    /// <summary>
    /// M�todo p�blico para a�adir una estrella a la UI y reproducir sonido.
    /// </summary>
    /// <param name="worldPosition">La posici�n en el mundo donde se recolect� la estrella (opcional para animaciones futuras).</param>
    public void AddStar(Vector3 worldPosition)
    {
        starCount++;
        UpdateStarUI();

        // Reproducir sonido de recolecci�n
        if (collectSound != null)
            audioSource.PlayOneShot(collectSound);
        else
            Debug.LogWarning("No se ha asignado un clip de sonido de recolecci�n en StarUIManager.");

        // Verificar si se ha alcanzado el n�mero m�ximo de estrellas
        if (starCount >= maxStars)
        {
            EndGame();
        }
    }

    /// <summary>
    /// Actualiza la UI agregando una nueva estrella en el contenedor.
    /// </summary>
    private void UpdateStarUI()
    {
        if (starContainer != null && starUIPrefab != null)
        {
            Instantiate(starUIPrefab, starContainer);
        }
        else
        {
            Debug.LogWarning("Star Container o Star UI Prefab no est�n asignados en StarUIManager.");
        }
    }

    /// <summary>
    /// Finaliza el juego cuando se recolectan todas las estrellas.
    /// </summary>
    private void EndGame()
    {
        Debug.Log("�Juego terminado! Todas las estrellas han sido recolectadas.");
        SceneManager.LoadScene("MainMenu");
    }
}
