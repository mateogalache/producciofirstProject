using System.Collections;
using UnityEngine;

public class SquareColorChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float minChangeInterval = 1f;
    [SerializeField] private float maxChangeInterval = 3f;
    [SerializeField] private float minWhiteDuration = 0.05f;
    [SerializeField] private float maxWhiteDuration = 0.2f;
    [SerializeField] private float fadeDuration = 0.5f; // Time to transition

    private static float sharedChangeInterval;
    private static float sharedWhiteDuration;
    private static bool isInitialized = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!isInitialized)
        {
            SetNewSharedTimes();
            StartCoroutine(UpdateSharedTimesRoutine()); // Sync new times
            isInitialized = true;
        }

        StartCoroutine(ChangeColorRoutine());
    }

    private IEnumerator ChangeColorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(sharedChangeInterval);

            // Smooth fade to 115 opacity
            yield return StartCoroutine(FadeAlpha(80f / 255f));

            yield return new WaitForSeconds(sharedWhiteDuration);

            // Smooth fade back to 185 opacity
            yield return StartCoroutine(FadeAlpha(130f / 255f));
        }
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
            yield return null; // Wait for next frame
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetAlpha); // Ensure exact final value
    }

    private static void SetNewSharedTimes()
    {
        sharedChangeInterval = Random.Range(1f, 3f);
        sharedWhiteDuration = Random.Range(0.05f, 0.2f);
    }

    private IEnumerator UpdateSharedTimesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(sharedChangeInterval + sharedWhiteDuration);
            SetNewSharedTimes();
        }
    }
}
