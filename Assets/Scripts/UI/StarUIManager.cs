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
    private int starCount = 0; 
    private int maxStars = 14; 

    private List<GameObject> collectedStars = new List<GameObject>(); 
    private Vector3[] targetPositions;
    private bool animateStars = false;

    private Vector3 centerScreen;

    private LineRenderer lineRenderer;

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        centerScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        //targetPositions = GetTargetPositionsForAndy(); // Guardamos las posiciones finales de la animación

        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 2.0f;
        lineRenderer.endWidth = 2.0f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.white;
        lineRenderer.enabled = false;
        
    }

    /// <summary>
    /// Método público para añadir una estrella a la UI y reproducir sonido.
    /// </summary>
    /// <param name="worldPosition">La posición en el mundo donde se recolectó la estrella (opcional para animaciones futuras).</param>
    public void AddStar(Vector3 worldPosition)
    {
        starCount++;
        UpdateStarUI();

        // Reproducir sonido de recolección
        if (collectSound != null)
            audioSource.PlayOneShot(collectSound);
        else
            Debug.LogWarning("No se ha asignado un clip de sonido de recolección en StarUIManager.");

        // Verifiquem si hem arribat al màxim d'estrelles
        if (starCount >= maxStars)
        {
            
            animateStars = true; 
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
        }
        else
        {
            Debug.LogWarning("Star Container o Star UI Prefab no están asignados en StarUIManager.");
        }
    }

    void Update()
    {
        if (targetPositions == null || targetPositions.Length == 0)
        {
            if (Screen.width > 0 && Screen.height > 0)
            {
                targetPositions = GetTargetPositionsForAndy();
            }
        }

        if (animateStars)
        {
            MoveStarsToFormName();
            
        }
    }

    private void MoveStarsToFormName()
    {
        bool allArrived = true;

        //bucle per recorrer totes les estrelles i mou cadascuna fins destí
        for (int i = 0; i < collectedStars.Count && i < targetPositions.Length; i++)
        {
            GameObject star = collectedStars[i];
            Vector3 targetPos = targetPositions[i];
            float step = 300f * Time.deltaTime;
            star.transform.position = Vector3.MoveTowards(star.transform.position, targetPos, step);

            if (star.transform.position != targetPositions[i])
                allArrived = false;

        }

        if (allArrived)
        {
            animateStars = false;
            Invoke("DrawLines", 0.1f); // Dibuja las líneas una sola vez después de que las estrellas lleguen
            //DrawLines();// Solo dibujamos las líneas cuando todas las estrellas están en su lugar
        }
    }

    private void DrawLines()
    {
        if (collectedStars.Count < maxStars)
        {
            return;
        }

        // Definir los índices de las estrellas que forman cada letra
        int[][] letterIndices = new int[][]
        {
        //new int[] {0, 1, 2},     // A
        //new int[] {3, 4, 5, 6},  // N
        //new int[] {7, 8, 9},   // D
        //new int[] {10, 11, 12, 13} // Y

            // A: 2 líneas (triángulo)
            new int[] { 0, 1, 2 }, // Formando una 'A' con dos líneas (1-2 y 2-3)

            // N: 3 líneas (dos verticales y una diagonal)
            new int[] { 3, 4, 5 }, // Formando la parte vertical
            new int[] { 4, 6 },     // Formando la diagonal

            // D: 3 líneas (una vertical y dos diagonales)
            new int[] { 7, 8 },     // Línea vertical
            new int[] { 8, 9 },     // Diagonal superior
            new int[] { 9, 10 },    // Diagonal inferior

            // Y: 3 líneas (dos diagonales y una vertical)
            new int[] { 11, 12 },   // Diagonal izquierda
            new int[] { 12, 13 },   // Diagonal derecha
            new int[] { 13, 14 }    // Línea vertical
          
        };

        // Crear un LineRenderer para cada letra
        foreach (var indices in letterIndices)
        {

            if (indices.Length == 0 || indices[indices.Length - 1] >= collectedStars.Count)
            {
                continue; // Evitar accesos fuera de rango
            }

            GameObject lineObject = new GameObject("LetterLine");
            LineRenderer letterLine = lineObject.AddComponent<LineRenderer>();
            //LineRenderer letterLine = new GameObject("LetterLine").AddComponent<LineRenderer>();

            letterLine.useWorldSpace = true;
            letterLine.startWidth = 1.0f;
            letterLine.endWidth = 1.0f;
            letterLine.material = new Material(Shader.Find("Unlit/Color"));
            letterLine.material.color = Color.white;
            letterLine.positionCount = indices.Length;
            //letterLine.sortingOrder = 10;

            for (int i = 0; i < indices.Length; i++)
            {
                int index = indices[i];

                if (index >= collectedStars.Count) continue; // Seguridad

                //letterLine.SetPosition(i, collectedStars[index].transform.position);

                Vector3 worldPos = collectedStars[index].transform.position;

                if (starContainer != null && starContainer.GetComponentInParent<Canvas>() != null)
                {
                    worldPos = Camera.main.ScreenToWorldPoint(new Vector3(worldPos.x, worldPos.y, Camera.main.nearClipPlane));
                    worldPos.z = 0;
                }

                letterLine.SetPosition(i, worldPos);

            }
            letterLine.enabled = true;

        }

    }

    //defineix totes les posicions in cada estrella ha de moure's per formar la paraula
    private Vector3[] GetTargetPositionsForAndy()
    {
        float startX = Screen.width / 3;
        float startY = Screen.height / 2;

        return new Vector3[]
        {

            //A
            new Vector3(startX, startY, 0), 
            new Vector3(startX + 20, startY + 40, 0),
            new Vector3(startX + 40, startY, 0),

            // N - Estrellas formando una línea vertical, diagonal y vertical
            new Vector3(startX + 80, startY, 0), // Estrella 1 (vertical)
            new Vector3(startX + 80, startY + 40, 0), // Estrella 2 (vertical)
            new Vector3(startX + 120, startY, 0), // Estrella 3 (diagonal)
            new Vector3(startX + 120, startY + 40, 0), // Estrella 4 (diagonal)            
            

            // D - Triángulo horizontal con una estrella central
            new Vector3(startX + 160, startY, 0), // Estrella 1 (izquierda)
            new Vector3(startX + 160, startY + 40, 0), // Estrella 2 (derecha)
            new Vector3(startX + 200, startY + 20, 0), // Estrella 3 (punta superior)

            // Y - Triángulo boca abajo y un palo vertical
            new Vector3(startX + 240, startY + 40, 0), // Estrella 1 (parte superior izquierda)
            new Vector3(startX + 280, startY + 40, 0), // Estrella 2 (parte superior derecha)
            new Vector3(startX + 260, startY + 20, 0), // Estrella 3 (parte inferior)
            new Vector3(startX + 260, startY, 0), // Estrella 4 (palo de la Y)
            

        };

    }

}
