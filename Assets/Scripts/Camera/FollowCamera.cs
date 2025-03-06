using UnityEngine;

public class AdvancedCameraFollow : MonoBehaviour
{
    [Header("Target and Basic Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothTime = 0.3f;

    [Header("Look-Ahead Settings")]
    public float lookAheadFactor = 3f;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

    [Header("Camera Bounds (Optional)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Header("Camera Shake Settings")]
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;

    private Vector3 currentVelocity = Vector3.zero;
    private float lastTargetX;
    private Vector3 lookAheadPos = Vector3.zero;

    // Variable para detener el movimiento de la cámara
    private bool stopCameraMovement = false;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogWarning("No GameObject with tag 'Player' was found for the camera to follow.");
        }

        if (target != null)
            lastTargetX = target.position.x;
    }

    void LateUpdate()
    {
        if (target == null || stopCameraMovement)
            return; // No hacer nada si la cámara está detenida o no hay target

        float xMoveDelta = target.position.x - lastTargetX;
        bool updateLookAhead = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

        if (updateLookAhead)
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        else
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);

        Vector3 targetPosition = target.position + offset + lookAheadPos;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        if (shakeDuration > 0)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            newPos.x += shakeOffset.x;
            newPos.y += shakeOffset.y;
            shakeDuration -= Time.deltaTime;
        }

        if (useBounds)
        {
            newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        }

        transform.position = new Vector3(newPos.x, newPos.y, offset.z);
        lastTargetX = target.position.x;
    }

    // Método para detener o reanudar el movimiento de la cámara
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
