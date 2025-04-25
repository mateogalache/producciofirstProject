using UnityEngine;

public class CompleteUpDownMovement : MonoBehaviour
{
    [Header("Height Settings")]
    public float topHeight = 3f;            // Altura máxima respecto al origen
    public float bottomHeight = 1f;         // Altura mínima
    private Vector3 basePosition;           // Posición de referencia

    [Header("Timing Settings")]
    public float initialDelay = 2f;         // Tiempo antes de empezar el movimiento (NUEVO)
    public float waitTimeAtTop = 1.5f;      // Espera al llegar arriba
    public float waitTimeAtBottom = 1f;     // Espera al llegar abajo
    public float moveDuration = 2f;         // Tiempo para subir/bajar
    public float shakeDuration = 0.6f;      // Temblor antes de bajar

    [Header("Shake Settings")]
    public float shakeIntensity = 0.15f;    // Fuerza del temblor
    public float shakeFrequency = 25f;      // Velocidad del temblor

    private float timer;
    private enum MovementPhase { Delay, GoingUp, WaitingAtTop, Shaking, GoingDown, WaitingAtBottom }
    private MovementPhase currentPhase;

    void Start()
    {
        basePosition = transform.position;
        currentPhase = MovementPhase.Delay;
        timer = initialDelay;

        // Posicionar inicialmente arriba (opcional, comentar si debe empezar abajo)
        // transform.position = basePosition + Vector3.up * topHeight;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        switch (currentPhase)
        {
            case MovementPhase.Delay:
                if (timer <= 0f) StartMovement();
                break;

            case MovementPhase.GoingUp:
                MoveToPosition(topHeight, MovementPhase.WaitingAtTop, waitTimeAtTop);
                break;

            case MovementPhase.WaitingAtTop:
                if (timer <= 0f)
                {
                    currentPhase = MovementPhase.Shaking;
                    timer = shakeDuration;
                }
                break;

            case MovementPhase.Shaking:
                ShakeEffect();
                if (timer <= 0f)
                {
                    currentPhase = MovementPhase.GoingDown;
                    timer = moveDuration;
                }
                break;

            case MovementPhase.GoingDown:
                MoveToPosition(bottomHeight, MovementPhase.WaitingAtBottom, waitTimeAtBottom);
                break;

            case MovementPhase.WaitingAtBottom:
                if (timer <= 0f)
                {
                    currentPhase = MovementPhase.GoingUp;
                    timer = moveDuration;
                }
                break;
        }
    }

    void StartMovement()
    {
        currentPhase = MovementPhase.GoingUp;
        timer = moveDuration;
    }

    void MoveToPosition(float targetHeight, MovementPhase nextPhase, float nextTimer)
    {
        float startHeight = (currentPhase == MovementPhase.GoingUp) ? bottomHeight : topHeight;
        float progress = 1f - (timer / moveDuration);

        // Movimiento suave con SmoothStep
        float smoothedProgress = Mathf.SmoothStep(0f, 1f, progress);
        float newY = Mathf.Lerp(startHeight, targetHeight, smoothedProgress) + basePosition.y;

        transform.position = new Vector3(basePosition.x, newY, basePosition.z);

        if (timer <= 0f)
        {
            currentPhase = nextPhase;
            timer = nextTimer;
        }
    }

    void ShakeEffect()
    {
        // Temblor con ruido Perlin (más natural)
        float noiseX = Mathf.PerlinNoise(Time.time * shakeFrequency, 0) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(0, Time.time * shakeFrequency) * 2f - 1f;

        Vector3 shakeOffset = new Vector3(noiseX, noiseY, 0) * shakeIntensity;
        transform.position = basePosition + Vector3.up * topHeight + shakeOffset;
    }

    // Reinicia el ciclo (útil para activar desde otros scripts)
    public void ResetCycle(float newDelay = -1f)
    {
        if (newDelay >= 0) initialDelay = newDelay;
        currentPhase = MovementPhase.Delay;
        timer = initialDelay;
        transform.position = basePosition;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) basePosition = transform.position;

        Gizmos.color = Color.cyan;
        Vector3 topPos = basePosition + Vector3.up * topHeight;
        Vector3 bottomPos = basePosition + Vector3.up * bottomHeight;

        // Línea con marcadores
        Gizmos.DrawLine(topPos, bottomPos);
        Gizmos.DrawWireCube(topPos, Vector3.one * 0.3f);
        Gizmos.DrawWireCube(bottomPos, Vector3.one * 0.3f);

        // Etiquetas informativas
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.cyan;
        UnityEditor.Handles.Label(topPos + Vector3.right * 0.5f,
            $"↑ Espera: {waitTimeAtTop}s\n↓ Temblor: {shakeDuration}s", style);
        UnityEditor.Handles.Label(bottomPos + Vector3.right * 0.5f,
            $"↑ Espera: {waitTimeAtBottom}s", style);
        UnityEditor.Handles.Label(basePosition + Vector3.right * 0.5f,
            $"Delay inicial: {initialDelay}s", style);
#endif
    }
}