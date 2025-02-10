using UnityEngine;

public class AdvancedCameraFollow : MonoBehaviour
{
    [Header("Target and Basic Settings")]
    // The target (usually the player) to follow.
    public Transform target;
    // Base offset from the target’s position (e.g., to adjust vertical framing).
    public Vector3 offset = new Vector3(0, 5, -10);
    // Time for the camera to catch up to the target.
    public float smoothTime = 0.3f;

    [Header("Look-Ahead Settings")]
    // How far ahead (in world units) the camera looks based on player's movement.
    public float lookAheadFactor = 3f;
    // How fast the look-ahead offset returns to zero when the player stops.
    public float lookAheadReturnSpeed = 0.5f;
    // Minimum movement threshold to trigger look-ahead.
    public float lookAheadMoveThreshold = 0.1f;

    [Header("Camera Bounds (Optional)")]
    // Enable bounds if you want to restrict the camera movement.
    public bool useBounds = false;
    // Minimum (bottom-left) and maximum (top-right) boundaries.
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Header("Camera Shake Settings (Triggered via ShakeCamera)")]
    // (These values will be set when shaking.)
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;

    // Private variables for smooth damp and look-ahead calculation.
    private Vector3 currentVelocity = Vector3.zero;
    private float lastTargetX;
    private Vector3 lookAheadPos = Vector3.zero;

    void Start()
    {
        // If no target is assigned, try to find one tagged as "Player".
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogWarning("No GameObject with tag 'Player' was found for the camera to follow.");
        }

        // Initialize the lastTargetX for look-ahead calculations.
        if (target != null)
            lastTargetX = target.position.x;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate how much the target has moved horizontally since the last frame.
        float xMoveDelta = target.position.x - lastTargetX;
        bool updateLookAhead = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

        // Update look-ahead position based on movement direction.
        if (updateLookAhead)
        {
            // Look ahead in the direction of movement.
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        }
        else
        {
            // Gradually move the look-ahead offset back to zero when not moving significantly.
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        }

        // Determine the desired camera position.
        Vector3 targetPosition = target.position + offset + lookAheadPos;

        // Smoothly move the camera towards the target position.
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // If camera shake is active, add a random offset.
        if (shakeDuration > 0)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            newPos.x += shakeOffset.x;
            newPos.y += shakeOffset.y;
            shakeDuration -= Time.deltaTime;
        }

        // Clamp the camera position if bounds are enabled.
        if (useBounds)
        {
            newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        }

        // Set the new camera position while preserving the desired z (usually the offset's z).
        transform.position = new Vector3(newPos.x, newPos.y, offset.z);

        // Update the last known target X position for the next frame.
        lastTargetX = target.position.x;
    }

    /// <summary>
    /// Triggers a camera shake effect.
    /// Call this method from other scripts (e.g., when the player takes damage).
    /// </summary>
    /// <param name="duration">Duration of the shake in seconds.</param>
    /// <param name="magnitude">Magnitude of the shake.</param>
    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
