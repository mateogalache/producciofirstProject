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

    [Header("Line")]
    public GameObject linePrefab;
    private List<GameObject> lines = new List<GameObject>();

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

        GridLayoutGroup grid = starContainer.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.cellSize = new Vector2(30, 30);
            grid.spacing = new Vector2(5, 5);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 7;
        }

        RectTransform containerRect = starContainer.GetComponent<RectTransform>();

        containerRect.anchorMin = new Vector2(0f, 1f);
        containerRect.anchorMax = new Vector2(0f, 1f);
        containerRect.pivot = new Vector2(0f, 1f);
        //containerRect.anchoredPosition = new Vector2(20,-20);
        //containerRect.sizeDelta = new Vector2(350, 100);
        containerRect.anchoredPosition = new Vector2(30f, -30f); // ¡OJO! Y es negativo porque el pivot es top-left
        containerRect.sizeDelta = new Vector2(300f, 100f);


    }

    /// <summary>
    /// Método público para añadir una estrella a la UI y reproducir sonido.
    /// </summary>
    /// <param name="worldPosition">La posición en el mundo donde se recolectó la estrella (opcional para animaciones futuras).</param>
    public void AddStar(Vector3 worldPosition)
    {
        starCount++;
        Debug.Log($"Estrella añadida. Total recogidas: {starCount}/{maxStars}");
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
            GridLayoutGroup grid = starContainer.GetComponent<GridLayoutGroup>();
            if (grid != null)
                Destroy(grid);
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
            RectTransform starRect = newStar.GetComponent<RectTransform>();
            //starRect.anchoredPosition = Vector2.zero; // o la posición inicial que quieras
            starRect.localScale = Vector3.one * 1.5f;
            starRect.anchoredPosition = Vector2.zero;
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
            //star.transform.position = Vector3.MoveTowards(star.transform.position, targetPos, step);
            RectTransform starRect = star.GetComponent<RectTransform>();
            starRect.anchoredPosition = Vector2.MoveTowards(starRect.anchoredPosition, targetPos, step);

            //if (star.transform.position != targetPositions[i])
            //    allArrived = false;
            //float distance = Vector3.Distance(star.transform.position, targetPositions[i]);
            float distance = Vector2.Distance(starRect.anchoredPosition, targetPositions[i]);
            if (distance > 0.1f)
            {
                allArrived = false;
                Debug.Log($"Estrella {i} aún no ha llegado. Distancia restante: {distance}");
            }
            else
            {
                Debug.Log($"Estrella {i} ha llegado al destino.");
            }

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
            Debug.Log("Aún no hay suficientes estrellas. Se necesitan " + maxStars);
            return;
        }

        // Definir los índices de las estrellas que forman cada letra
        int[][] connections = new int[][]
        {


            new int[] {0, 1}, new int[] {1, 2}, // A
            new int[] {3, 4}, new int[] {4, 5}, new int[] {5, 6}, // N
            new int[] {7, 8}, new int[] {8, 9}, new int[] {9, 7}, // D
            new int[] {10, 12}, new int[] {11, 12}, new int[] {12, 13}, // Y
        };

        foreach (var pair in connections)
        {
            if (pair.Length != 2) continue;

            GameObject startStar = collectedStars[pair[0]];
            GameObject endStar = collectedStars[pair[1]];


            //Vector2 startPos = startStar.GetComponent<RectTransform>().anchoredPosition;
            //Vector2 endPos = endStar.GetComponent<RectTransform>().anchoredPosition;
            Vector2 startPos = startStar.GetComponent<RectTransform>().localPosition;
            Vector2 endPos = endStar.GetComponent<RectTransform>().localPosition;


            CreateUILine(startPos, endPos);
        }


    }

    private void CreateUILine(Vector3 start, Vector3 end)
    {
        GameObject line = Instantiate(linePrefab, starContainer); // Dentro del canvas
        RectTransform rect = line.GetComponent<RectTransform>();

        //rect.sizeDelta = new Vector2(6f, length); // grosor, largo
        //rect.pivot = new Vector2(0.5f, 0f);
        //rect.position = start;
        //rect.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        line.transform.SetParent(starContainer, false); // mantiene escala y posicionamiento local
        line.transform.SetAsLastSibling(); // asegura que la línea se dibuje por encima

        Vector3 dir = end - start;
        float length = dir.magnitude;

        // Ajustes visuales
        rect.sizeDelta = new Vector2(6f, length); // ancho y largo de la línea
        rect.pivot = new Vector2(0.5f, 0f);

        // Ajuste de posición dentro del Canvas
        //Vector3 localStart = starContainer.InverseTransformPoint(start);
        //rect.localPosition = localStart;

        //Vector2 anchoredStart = start;
        //rect.anchoredPosition = anchoredStart;

        rect.localPosition = start;

        //Debug.Log($"Línea creada desde: {start} hasta: {end} | Largo: {length}");
        //rect.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        rect.localRotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);

        lines.Add(line);

        
        Image img = line.GetComponent<Image>();
        if (img != null)
        {
            img.enabled = true;
            img.color = Color.white;
        }
        Debug.Log("Creating line at: " + start);
        
    }


    //defineix totes les posicions in cada estrella ha de moure's per formar la paraula
    private Vector3[] GetTargetPositionsForAndy()
    {

        /*
        RectTransform canvasRect = starContainer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        float centerX = canvasRect.rect.width / 2f;
        float centerY = canvasRect.rect.height / 2f;

        float offsetX = 500f;



        return new Vector3[]
        {
        // A
        new Vector3(-140 + offsetX, -20),
        new Vector3(-120 + offsetX,  20),
        new Vector3(-100 + offsetX, -20), 

        // N
        new Vector3(-60 + offsetX, -20),
        new Vector3(-60 + offsetX, 20),
        new Vector3(-20 + offsetX, -20),
        new Vector3(-20 + offsetX, 20), 

        // D
        new Vector3(20 + offsetX, -20),
        new Vector3(20 + offsetX, 20),
        new Vector3(60 + offsetX, 0), 

        // Y
        new Vector3(100 + offsetX, 20),
        new Vector3(140 + offsetX, 20),
        new Vector3(120 + offsetX, 0),
        new Vector3(120 + offsetX, -20)
        };*/

        RectTransform containerRect = starContainer.GetComponent<RectTransform>();

        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        Vector2 containerCenter = new Vector2(containerWidth / 2f, -containerHeight / 2f); // Y negativo porque pivot es top-left

        // Posiciones relativas al centro del contenedor para formar la palabra "ANDY"
        Vector3[] positions = new Vector3[]
        {
        // A
        new Vector3(-140, -20), new Vector3(-120, 20), new Vector3(-100, -20),

        // N
        new Vector3(-60, -20), new Vector3(-60, 20), new Vector3(-20, -20), new Vector3(-20, 20),

        // D
        new Vector3(20, -20), new Vector3(20, 20), new Vector3(60, 0),

        // Y
        new Vector3(100, 20), new Vector3(140, 20), new Vector3(120, 0), new Vector3(120, -20)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] += (Vector3)containerCenter;
        }

        return positions;
    }

}