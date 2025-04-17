using System.Collections;
using UnityEngine;

public class RandomElevatorAbsolute : MonoBehaviour
{
    [Header("Intervalo entre elevaciones (aleatorio)")]
    [Tooltip("Tiempo m�nimo a esperar antes de elevarse")]
    public float minInterval = 2f;
    [Tooltip("Tiempo m�ximo a esperar antes de elevarse")]
    public float maxInterval = 5f;

    [Header("Destino y permanencia")]
    [Tooltip("Altura absoluta a la que se elevar�")]
    public float targetY = 135f;
    [Tooltip("Tiempo que permanece arriba")]
    public float holdDuration = 1f;

    [Header("Duraciones aleatorias de movimiento")]
    [Tooltip("Duraci�n m�nima para subir")]
    public float minUpDuration = 0.5f;
    [Tooltip("Duraci�n m�xima para subir")]
    public float maxUpDuration = 2f;
    [Tooltip("Duraci�n m�nima para bajar")]
    public float minDownDuration = 0.5f;
    [Tooltip("Duraci�n m�xima para bajar")]
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

            // Duraci�n de subida aleatoria
            float upDuration = Random.Range(minUpDuration, maxUpDuration);
            yield return StartCoroutine(MoveToPosition(upPos, upDuration));

            // Mantener arriba
            yield return new WaitForSeconds(holdDuration);

            // Duraci�n de bajada aleatoria
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
