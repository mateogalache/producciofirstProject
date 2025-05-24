using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StarUIManager : MonoBehaviour
{
    public static StarUIManager Instance;

    [Header("UI Elements")]
    public Transform starContainer;
    public GameObject starUIPrefab;

    [Header("Line")]
    public GameObject linePrefab;
    private List<GameObject> lines = new List<GameObject>();

    [Header("Audio")]
    public AudioClip collectSound;
    private AudioSource audioSource;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;
    public float delayBeforeFade = 2f;

    private int starCount = 0;
    private int maxStars = 14;

    private List<GameObject> collectedStars = new List<GameObject>();
    private Vector3[] targetPositions;
    private bool animateStars = false;

    private Vector3 centerScreen;

    private bool hasRestoredUI = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

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
        containerRect.anchoredPosition = new Vector2(10f, -10f);
        containerRect.sizeDelta = new Vector2(300f, 100f);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f; 
            fadeImage.color = c;

            StartCoroutine(FadeFromBlack());
        }

        if (hasRestoredUI) return;
        hasRestoredUI = true;

        int savedStars = StarTracker.Instance.collectedStars;
        for (int i = 0; i < savedStars; i++)
        {
            UpdateStarUI();
        }
    }

    public void AddStar(Vector3 worldPosition)
    {
        Debug.Log("[StarUIManager] UI: Añadiendo estrella visual");

        GameObject newStar = Instantiate(starUIPrefab, starContainer);
        RectTransform starRect = newStar.GetComponent<RectTransform>();
        starRect.localScale = Vector3.one;
        starRect.anchoredPosition = Vector2.zero;
        collectedStars.Add(newStar);

        if (collectSound != null)
            audioSource.PlayOneShot(collectSound, 0.2f);

      
        if (StarTracker.Instance.collectedStars >= StarTracker.Instance.maxStars && !animateStars)
        {
            Debug.Log("[StarUIManager] Todas las estrellas recogidas. Activando animación final.");
            var grid = starContainer.GetComponent<GridLayoutGroup>();
            if (grid != null) Destroy(grid);
            animateStars = true;
        }
    }


    private void UpdateStarUI()
    {
        if (starContainer != null && starUIPrefab != null)
        {
            GameObject newStar = Instantiate(starUIPrefab, starContainer);
            RectTransform starRect = newStar.GetComponent<RectTransform>();
            starRect.localScale = Vector3.one;
            starRect.anchoredPosition = Vector2.zero;
            collectedStars.Add(newStar);
        }
        else
        {
            Debug.LogWarning("Star Container o Star UI Prefab no están asignados.");
        }
    }

    void Update()
    {
        if (targetPositions == null || targetPositions.Length == 0)
        {
            if (Screen.width > 0 && Screen.height > 0)
                targetPositions = GetTargetPositionsForAndy();
        }

        if (animateStars) MoveStarsToFormName();
    }

    private void MoveStarsToFormName()
    {
        bool allArrived = true;

        for (int i = 0; i < collectedStars.Count && i < targetPositions.Length; i++)
        {
            GameObject star = collectedStars[i];
            Vector3 targetPos = targetPositions[i];
            float step = 300f * Time.deltaTime;
            RectTransform starRect = star.GetComponent<RectTransform>();
            starRect.anchoredPosition = Vector2.MoveTowards(starRect.anchoredPosition, targetPos, step);

            if (Vector2.Distance(starRect.anchoredPosition, targetPos) > 0.1f)
            {
                allArrived = false;
            }
        }

        if (allArrived)
        {
            animateStars = false;
            Debug.Log("Todas las estrellas llegaron. Dibuja líneas.");
            DrawLines();
            StartCoroutine(DelayThenFadeToScene("Mirror"));
        }
    }

    private IEnumerator DelayThenFadeToScene(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeFade);
        yield return StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadeImage != null)
        {
            Color startColor = fadeImage.color;
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0f, 0f, 0f, alpha); 
            }
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    private void DrawLines()
    {
        if (collectedStars.Count < maxStars) return;

        int[][] connections = new int[][]
        {
            new int[] {0, 1}, new int[] {1, 2},
            new int[] {3, 4}, new int[] {4, 5}, new int[] {5, 6},
            new int[] {7, 8}, new int[] {8, 9}, new int[] {9, 7},
            new int[] {10, 12}, new int[] {11, 12}, new int[] {12, 13},
        };

        foreach (var pair in connections)
        {
            if (pair.Length != 2) continue;

            GameObject startStar = collectedStars[pair[0]];
            GameObject endStar = collectedStars[pair[1]];

            Vector2 startPos = startStar.GetComponent<RectTransform>().localPosition;
            Vector2 endPos = endStar.GetComponent<RectTransform>().localPosition;

            CreateUILine(startPos, endPos);
        }
    }

    private void CreateUILine(Vector3 start, Vector3 end)
    {
        GameObject line = Instantiate(linePrefab, starContainer);
        RectTransform rect = line.GetComponent<RectTransform>();
        line.transform.SetParent(starContainer, false);
        line.transform.SetAsLastSibling();

        Vector3 dir = end - start;
        float length = dir.magnitude;

        rect.sizeDelta = new Vector2(6f, length);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.localPosition = start;
        rect.localRotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);

        lines.Add(line);

        Image img = line.GetComponent<Image>();
        if (img != null)
        {
            img.enabled = true;
            img.color = Color.white;
        }
    }

    private Vector3[] GetTargetPositionsForAndy()
    {
        RectTransform containerRect = starContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        Vector2 containerCenter = new Vector2(containerWidth / 2f, -containerHeight / 2f);

        Vector3[] positions = new Vector3[]
        {
            new Vector3(-140, -20), new Vector3(-120, 20), new Vector3(-100, -20),
            new Vector3(-60, -20), new Vector3(-60, 20), new Vector3(-20, -20), new Vector3(-20, 20),
            new Vector3(20, -20), new Vector3(20, 20), new Vector3(60, 0),
            new Vector3(100, 20), new Vector3(140, 20), new Vector3(120, 0), new Vector3(120, -20)
        };

        for (int i = 0; i < positions.Length; i++)
            positions[i] += (Vector3)containerCenter;

        return positions;
    }
}
