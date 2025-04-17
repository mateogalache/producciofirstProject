using UnityEngine;

public class AdvancedCameraFollow : MonoBehaviour
{
    [Header("Target and Basic Settings")]
    public Transform target;
    [Tooltip("Offset base de la cámara")]
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothTime = 0.3f;

    [Header("Glide Camera Settings")]
    [Tooltip("Cuando planea, la cámara baja este valor en Y")]
    public float glideYOffset = -2f;
    [Tooltip("Velocidad para suavizar el cambio de offset")]
    public float offsetSmoothSpeed = 2f;

    [Header("Look-Ahead Settings")]
    public float lookAheadFactor = 3f;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

    [Header("Camera Bounds (Optional)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    // Shake
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;

    Vector3 currentVelocity = Vector3.zero;
    float lastTargetX;
    Vector3 lookAheadPos = Vector3.zero;
    bool stopCameraMovement = false;

    // Para manejar dinámicamente la Y del offset
    float currentOffsetY;
    PlayerMovementLvl2 playerMovement;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogWarning("No hay GameObject con tag 'Player' para seguir.");
        }

        if (target != null)
        {
            lastTargetX = target.position.x;
            playerMovement = target.GetComponent<PlayerMovementLvl2>();
        }

        // Inicializamos el offset Y actual al valor base
        currentOffsetY = offset.y;
    }

    void LateUpdate()
    {
        if (target == null || stopCameraMovement) return;

        // 1) Ajuste dinámico de Y según planeo
        bool gliding = playerMovement != null && playerMovement.IsGliding;
        float desiredY = offset.y + (gliding ? glideYOffset : 0f);
        currentOffsetY = Mathf.Lerp(currentOffsetY, desiredY, Time.deltaTime * offsetSmoothSpeed);

        // 2) LookAhead horizontal
        float xMoveDelta = target.position.x - lastTargetX;
        bool updateLA = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
        if (updateLA)
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        else
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);

        // 3) Construir posición objetivo
        Vector3 dynamicOffset = new Vector3(offset.x, currentOffsetY, offset.z);
        Vector3 targetPos = target.position + dynamicOffset + lookAheadPos;

        // 4) Suavizar movimiento
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);

        // 5) Shake si toca
        if (shakeDuration > 0)
        {
            Vector2 shakeOff = Random.insideUnitCircle * shakeMagnitude;
            newPos.x += shakeOff.x;
            newPos.y += shakeOff.y;
            shakeDuration -= Time.deltaTime;
        }

        // 6) Aplicar límites
        if (useBounds)
        {
            newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        }

        transform.position = new Vector3(newPos.x, newPos.y, offset.z);
        lastTargetX = target.position.x;
    }

    public void SetCameraMovement(bool canMove)
    {
        stopCameraMovement = !canMove;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
