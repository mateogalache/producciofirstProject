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

    //private LineRenderer lineRenderer;

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

        //if (lineRenderer == null)
        //{
        //    lineRenderer = gameObject.AddComponent<LineRenderer>();
        //}

        //targetPositions = GetTargetPositionsForAndy(); // Guardamos las posiciones finales de la animación

        //lineRenderer.useWorldSpace = true;
        //lineRenderer.startWidth = 2.0f;
        //lineRenderer.endWidth = 2.0f;
        //lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        //lineRenderer.material.color = Color.white;
        //lineRenderer.enabled = false;

        /////////////////////
        ///lineRenderer.positionCount = 2;
        //lineRenderer.SetPosition(0, new Vector3(0, 0, -1f));
        //lineRenderer.SetPosition(1, new Vector3(300, 300, -1f));
        //lineRenderer.enabled = true;

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
            Debug.Log("Se han recogido todas las estrellas. Inicia animación.");
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
            Debug.Log("Todas las estrellas llegaron a destino. Se invoca DrawLines.");
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

            new int[] { 0, 1, 2, 0 }, //A

            //N
            new int[] { 3, 4 }, // Vertical izquierda
            new int[] { 4, 5 }, // Diagonal
            new int[] { 5, 6 },

            //D
            new int[] { 7, 8 }, // Vertical
            new int[] { 8, 9 }, // Diagonal superior
            new int[] { 9, 7 }, // Diagonal inferior cerrando forma curva

            //Y
            new int[] { 10, 12 }, // Diagonal izquierda
            new int[] { 11, 12 }, // Diagonal derecha
            new int[] { 12, 13 }, // Vertical central
        };

        Canvas canvas = starContainer.GetComponentInParent<Canvas>();
        Camera cam = Camera.main;


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
            letterLine.startWidth = 2f;
            letterLine.endWidth = 2f;
            //letterLine.material = new Material(Shader.Find("Unlit/Color"));
            //letterLine.material.color = Color.white;
            letterLine.material = new Material(Shader.Find("Sprites/Default"));
            letterLine.positionCount = indices.Length;
            //letterLine.sortingLayerName = "Default";
            //letterLine.sortingOrder = 100;
            letterLine.sortingOrder = 10;

           
            for (int i = 0; i < indices.Length; i++)
            {
                int index = indices[i];

                if (index >= collectedStars.Count) continue; // Seguridad

                Vector3 uiPosition = collectedStars[index].GetComponent<RectTransform>().position;

                //Vector3 worldPos = collectedStars[index].transform.position;
                Vector3 worldPos;

                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    //worldPos = cam.ScreenToWorldPoint(new Vector3(uiPosition.x, uiPosition.y, 10f)); // Ajusta Z si es necesario
                    worldPos = collectedStars[index].transform.position;
                } else
                {
                    Vector3 uiPos = collectedStars[index].GetComponent<RectTransform>().position;
                    worldPos = cam.ScreenToWorldPoint(new Vector3(uiPos.x, uiPos.y, 10f));
                }
                    /*
                    if (starContainer != null && starContainer.GetComponentInParent<Canvas>() != null)
                    {
                        worldPos = Camera.main.ScreenToWorldPoint(new Vector3(worldPos.x, worldPos.y, Camera.main.nearClipPlane));
                        worldPos.z = 0;
                    }
                    */
                worldPos.z = 0f;
                letterLine.SetPosition(i, worldPos);

                // Si el canvas es World Space, no necesitas convertir nada
                //worldPos = collectedStars[index].transform.position;


                //letterLine.SetPosition(i, worldPos);
                //Debug.DrawLine(worldPos, worldPos + Vector3.up * 10f, Color.red, 2f);
            
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
