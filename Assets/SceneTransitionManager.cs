using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        /*
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/

        // Verifica si ya existe uno
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Verifica si es root. Si no lo es, muestra advertencia y destruye.
        if (transform.parent != null)
        {
            Debug.LogWarning("SceneTransitionManager debe estar en la raíz del Hierarchy.");
            Destroy(gameObject);
            return;
        }

        // Persistencia segura entre escenas
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Empieza con fade in
        if (fadeImage != null)
        {

            fadeImage.raycastTarget = false;

            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            StartCoroutine(FadeFromBlack());
        }

        // Escucha el evento de nueva escena cargada para volver a hacer fade in
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = false;
            StartCoroutine(FadeFromBlack());
        }
    }

    public void LoadSceneWithFade(string sceneName, float delay = 0f)
    {
        StartCoroutine(FadeAndLoad(sceneName, delay));
    }

    IEnumerator FadeAndLoad(string sceneName, float delay)
    {
        if (fadeImage == null)
        {
            Debug.LogError("fadeImage no asignado en SceneTransitionManager.");
            yield break;
        }

        fadeImage.raycastTarget = true;

        yield return new WaitForSeconds(delay);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeFromBlack()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Asegura que no bloquee después del fade
        if (fadeImage != null)
            fadeImage.raycastTarget = false;
    }
}
