using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic; //

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
    [Tooltip("Clip de audio que se reproducirá al recolectar una estrella")]
    public AudioClip collectSound;

    private AudioSource audioSource;
    private int starCount = 0; //contador de estrellas recolectadas
    private int maxStars = 14; // Número máximo de estrellas para terminar el juego

    private List<GameObject> collectedStars = new List<GameObject>(); //Lista que almacena las estrellas recolectadas
    private Vector3[] targetPositions; //posiciones finales para formar la palabra ANDY
    private bool animateStars = false; //booleano que indica si la animació se activa


    void Awake()
    {
        // Implementar el patrón Singleton
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
        // Obtener o añadir un AudioSource para reproducir sonidos
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        targetPositions = GetTargetPositionsForAndy(); // Guardamos las posiciones finales de la animación


        // Verificar asignaciones
        if (starContainer == null)
            Debug.LogWarning("Star Container no está asignado en StarUIManager.");

        if (starUIPrefab == null)
            Debug.LogWarning("Star UI Prefab no está asignado en StarUIManager.");
    }

    /// <summary>
    /// Método público para añadir una estrella a la UI y reproducir sonido.
    /// </summary>
    /// <param name="worldPosition">La posición en el mundo donde se recolectó la estrella (opcional para animaciones futuras).</param>
    public void AddStar(Vector3 worldPosition)
    {
        starCount++;
        UpdateStarUI();

        //GameObject newStar = Instantiate(starUIPrefab, starContainer);
        //collectedStars.Add(newStar);

        // Reproducir sonido de recolección
        if (collectSound != null)
            audioSource.PlayOneShot(collectSound);
        else
            Debug.LogWarning("No se ha asignado un clip de sonido de recolección en StarUIManager.");

        // Verificar si se ha alcanzado el número máximo de estrellas
        if (starCount >= maxStars)
        {
            /*EndGame();*/
            animateStars = true; // Activar la animación en el Update
        }
    }

    /// <summary>
    /// Actualiza la UI agregando una nueva estrella en el contenedor.
    /// </summary>
    private void UpdateStarUI()
    {
        if (starContainer != null && starUIPrefab != null)
        {
            GameObject newStar = Instantiate(starUIPrefab, starContainer);
            collectedStars.Add(newStar);
            //Instantiate(starUIPrefab, starContainer);
        }
        else
        {
            Debug.LogWarning("Star Container o Star UI Prefab no están asignados en StarUIManager.");
        }
    }

    void Update() 
    {
        if (animateStars)
        {
            MoveStarsToFormName();
        }
    }

    private void MoveStarsToFormName()
    {
        //bucle per recorrer totes les estrelles i mou cadascuna fins destí
        for (int i = 0; i < collectedStars.Count && i < targetPositions.Length; i++)
        {
            GameObject star = collectedStars[i];
            Vector3 targetPos = targetPositions[i];

            float step = 200f * Time.deltaTime;
            star.transform.position = Vector3.MoveTowards(star.transform.position, targetPos, step);
        }

        //verifiquem si totes les estrelles han arribat a la posicio
        bool allStarsArrived = true;
        for (int i = 0; i < collectedStars.Count && i < targetPositions.Length; i++)
        {
            if (collectedStars[i].transform.position != targetPositions[i])
            {
                allStarsArrived = false;
                break;
            }
        }

        if (allStarsArrived)
        {
            animateStars = false;
            Debug.Log("Animación final completada.");
        }
    }

    //defineix totes les posicions in cada estrella ha de moure's per formar la paraula
    private Vector3[] GetTargetPositionsForAndy()
    {
        return new Vector3[]
        {
            new Vector3(-100, 50, 0), new Vector3(-90, 70, 0), new Vector3(-80, 50, 0), // "A"
            new Vector3(-60, 70, 0), new Vector3(-60, 50, 0),                           // "N"
            new Vector3(-30, 60, 0), new Vector3(-20, 50, 0), new Vector3(-10, 60, 0),  // "D"
            new Vector3(10, 70, 0), new Vector3(15, 50, 0), new Vector3(20, 70, 0),    // "Y"
        };
    }

    /*
    /// <summary>
    /// Finaliza el juego cuando se recolectan todas las estrellas.
    /// </summary>
    private void EndGame()
    {
        Debug.Log("¡Juego terminado! Todas las estrellas han sido recolectadas.");
        SceneManager.LoadScene("MainMenu");
    }
    */
}
