using System.Collections;
using UnityEngine;

public class RandomElevatorAbsolute : MonoBehaviour
{
    [Header("Intervalo entre elevaciones (aleatorio)")]
    [Tooltip("Tiempo mínimo a esperar antes de elevarse")]
    public float minInterval = 2f;
    [Tooltip("Tiempo máximo a esperar antes de elevarse")]
    public float maxInterval = 5f;

    [Header("Destino y permanencia")]
    [Tooltip("Altura absoluta a la que se elevará")]
    public float targetY = 135f;
    [Tooltip("Tiempo que permanece arriba")]
    public float holdDuration = 1f;

    [Header("Duraciones aleatorias de movimiento")]
    [Tooltip("Duración mínima para subir")]
    public float minUpDuration = 0.5f;
    [Tooltip("Duración máxima para subir")]
    public float maxUpDuration = 2f;
    [Tooltip("Duración mínima para bajar")]
    public float minDownDuration = 0.5f;
    [Tooltip("Duración máxima para bajar")]
    public float maxDownDuration = 2f;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
        StartCoroutine(ElevateRoutine());
    }

    private IEnumerator ElevateRoutine()
    {
        while (true)
        {
            // Espera un intervalo aleatorio
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            // Calcula objetivo absoluto
            Vector3 upPos = new Vector3(originalPos.x, targetY, originalPos.z);

            // Duración de subida aleatoria
            float upDuration = Random.Range(minUpDuration, maxUpDuration);
            yield return StartCoroutine(MoveToPosition(upPos, upDuration));

            // Mantener arriba
            yield return new WaitForSeconds(holdDuration);

            // Duración de bajada aleatoria
            float downDuration = Random.Range(minDownDuration, maxDownDuration);
            yield return StartCoroutine(MoveToPosition(originalPos, downDuration));
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }
}
