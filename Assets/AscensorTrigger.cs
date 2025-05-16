using UnityEngine;
using UnityEngine.SceneManagement;

public class AscensorTrigger : MonoBehaviour
{
    public float moveSpeed = 2f;                  // Velocidad del ascensor
    public float moveHeight = 5f;                 // Altura a la que se mueve
    public float delayBeforeSceneChange = 3f;     // Tiempo antes de cambiar de escena
    public string nextSceneName = "Level3";       // Escena a cargar

    private bool activated = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform ascensorTransform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigggggggger");
        if (!activated && other.CompareTag("Player"))
        {
            Debug.Log("Hola ascensor");
            activated = true;

            ascensorTransform = transform.parent;
            startPosition = ascensorTransform.position;
            targetPosition = startPosition + Vector3.up * moveHeight;

            StartCoroutine(MoverAscensorYTransicionar());
        }
    }

    private System.Collections.IEnumerator MoverAscensorYTransicionar()
    {
        float elapsed = 0f;
        while (elapsed < delayBeforeSceneChange)
        {
            ascensorTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / delayBeforeSceneChange);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ascensorTransform.position = targetPosition; // asegurar posición final
        SceneManager.LoadScene(nextSceneName);
    }
}
