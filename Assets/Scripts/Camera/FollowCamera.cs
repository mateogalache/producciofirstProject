using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The target the camera should follow.")]
    public Transform target;

    [Header("Offset Settings")]
    [Tooltip("Base offset from the target.")]
    public Vector3 offset = new Vector3(0, 5, -10);

    [Tooltip("Maximum offset for look-ahead based on target's movement.")]
    public Vector3 lookAheadOffset = new Vector3(3, 0, 0);

    [Header("Follow Settings")]
    [Tooltip("Speed at which the camera follows the target.")]
    public float followSpeed = 5f;

    [Tooltip("Time it takes for the camera to catch up to the target.")]
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private Vector3 currentLookAhead;
    private Vector3 desiredLookAhead;

    void Start()
    {
        // Automatically find the player if target is not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("No GameObject with tag 'Player' found. Please assign a target for the camera.");
            }
        }

        // Initialize look-ahead
        currentLookAhead = Vector3.zero;
        desiredLookAhead = Vector3.zero;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate look-ahead based on target's velocity or movement direction
        Vector3 targetVelocity = Vector3.zero;
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null)
        {
            targetVelocity = targetRb.velocity;
        }

        float lookAheadFactor = 1.5f;
        desiredLookAhead = new Vector3(Mathf.Sign(targetVelocity.x) * lookAheadOffset.x * lookAheadFactor, 0, 0);

        // Smoothly interpolate the look-ahead
        currentLookAhead = Vector3.Lerp(currentLookAhead, desiredLookAhead, Time.deltaTime * followSpeed);

        // Define the target position with offset and look-ahead
        targetPosition = target.position + offset + currentLookAhead;

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    /// <summary>
    /// Optional: Visualize the camera's look-ahead direction in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Vector3 lookAheadPoint = target.position + offset + desiredLookAhead;
            Gizmos.DrawSphere(lookAheadPoint, 0.3f);
        }
    }
}
