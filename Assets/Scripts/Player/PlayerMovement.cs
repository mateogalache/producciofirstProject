
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAudio))] // Ensure you have a PlayerAudio script (or remove if unused)
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationTime = 0.1f;   // Time to reach full speed when input is given
    [SerializeField] private float decelerationTime = 0.05f;    // Time to slow down when input is released
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer; // Assign the layer(s) that count as ground

    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;       // Allows jump shortly after leaving ground
    [SerializeField] private float jumpBufferTime = 0.2f;   // Buffers jump input before landing
    [Header("Carry Settings")]
    [SerializeField] private float jumpCarryMultiplier = 0.8f; // Adjust this value to control jump height when carrying an object


    [Header("Drag Settings")]
    [SerializeField] private Transform grabPoint;         // Child transform where the object will attach
    [SerializeField] private float grabRadius = 1f;         // Detection radius for draggable objects
    [SerializeField] private LayerMask draggableLayer;      // Should be set to the "Draggable" layer

    private Rigidbody2D rb;
    private PlayerAudio playerAudio;
    private Animator animator; // Optional – if you use animations

    private float horizontalInput;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float velocityXSmoothing;

    private bool isFacingRight = true;
    private bool canDoubleJump;

    // Reference to the currently grabbed object (if any)
    private DraggableObject grabbedObject;

    private Collider2D playerCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        playerAudio = GetComponent<PlayerAudio>();
        

        // Get the player's own Collider2D.
        playerCollider = GetComponent<Collider2D>();
    }


    void Update()
    {
        GetInput();
        HandleJumpBuffer();
        UpdateCoyoteTimer();
        HandleGrabInput();
        HandleFlip();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        Move();
        ApplyCustomGravity();
    }

    #region Input & Movement

    /// <summary>
    /// Reads horizontal and jump input.
    /// </summary>
    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates the coyote timer which allows a jump shortly after leaving the ground.
    /// </summary>
    private void UpdateCoyoteTimer()
    {
        if (IsGrounded())
        {
            coyoteTimer = coyoteTime;
            canDoubleJump = true;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks if a jump input was buffered and if conditions allow a jump.
    /// </summary>
    private void HandleJumpBuffer()
    {
        if (jumpBufferTimer > 0f)
        {
            if (coyoteTimer > 0f || canDoubleJump)
            {
                Jump();
                jumpBufferTimer = 0f;

                if (!IsGrounded() && canDoubleJump)
                    canDoubleJump = false;
            }
        }
    }

    /// <summary>
    /// Executes a jump. If a draggable object is held, the jump force is reduced based on its mass.
    /// </summary>
    private void Jump()
    {
        float finalJumpForce = jumpForce;

        // If an object is grabbed, apply the carry multiplier so the jump is slightly reduced.
        if (grabbedObject != null)
        {
            finalJumpForce = jumpForce * jumpCarryMultiplier;
        }

        rb.velocity = new Vector2(rb.velocity.x, finalJumpForce);
        playerAudio?.PlayJumpSound();
    }


    /// <summary>
    /// Handles horizontal movement using SmoothDamp for more refined acceleration and deceleration.
    /// </summary>
    private void Move()
    {
        // If carrying an object, reduce speed slightly.
        float currentSpeed = (grabbedObject != null) ? moveSpeed * 0.7f : moveSpeed;
        float targetSpeed = horizontalInput * currentSpeed;

        // Use a faster smoothing time when no input is provided (deceleration)
        float smoothTime = Mathf.Abs(horizontalInput) > 0.01f ? accelerationTime : decelerationTime;
        float newVelocityX = Mathf.SmoothDamp(rb.velocity.x, targetSpeed, ref velocityXSmoothing, smoothTime);
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
    }

    /// <summary>
    /// Applies a custom gravity multiplier for better jump control.
    /// </summary>
    private void ApplyCustomGravity()
    {
        if (rb.velocity.y < 0)
        {
            // Increase downward force when falling
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityScale * 2 - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Cut jump height when the jump button is released early
            rb.velocity += Vector2.up * Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Flips the character's facing direction based on movement.
    /// </summary>
    private void HandleFlip()
    {
        if ((horizontalInput > 0 && !isFacingRight) ||
            (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Updates animator parameters (if you use an Animator).
    /// </summary>
    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsGrounded", IsGrounded());
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
            animator.SetBool("IsCarrying", grabbedObject != null);
        }
    }

    #endregion

    #region Dragging

    /// <summary>
    /// Checks input for grabbing or releasing a draggable object.
    /// </summary>
    private void HandleGrabInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (grabbedObject == null)
            {
                TryGrab();
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (grabbedObject != null)
            {
                ReleaseGrab();
            }
        }
    }

    private void TryGrab()
    {
        if (grabPoint == null)
        {
            Debug.LogWarning("GrabPoint is not assigned on the player.");
            return;
        }

        Debug.Log("Attempting to grab object. GrabPoint position: " + grabPoint.position + " | GrabRadius: " + grabRadius);

        // Use OverlapCircleAll to check for colliders on the draggable layer.
        Collider2D[] hits = Physics2D.OverlapCircleAll(grabPoint.position, grabRadius, draggableLayer);

        if (hits.Length == 0)
        {
            Debug.Log("No colliders found in grab radius.");
            return;
        }

        Debug.Log("Found " + hits.Length + " collider(s) in grab range.");

        foreach (Collider2D col in hits)
        {
            // Use GetComponentInParent to search up the hierarchy for the DraggableObject script.
            DraggableObject dObj = col.GetComponentInParent<DraggableObject>();
            if (dObj != null)
            {
                grabbedObject = dObj;
                Debug.Log("Grabbing object: " + dObj.gameObject.name);
                grabbedObject.Grab(grabPoint);

                // Disable collisions between the player and the grabbed object.
                Collider2D boxCollider = grabbedObject.GetComponent<Collider2D>();
                if (playerCollider != null && boxCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, boxCollider, true);
                    Debug.Log("Ignoring collisions between player and " + grabbedObject.gameObject.name);
                }
                return;
            }
            else
            {
                Debug.Log("Collider " + col.name + " does not have a DraggableObject component in its parent hierarchy.");
            }
        }

        Debug.Log("No draggable object found among colliders.");
    }




    /// <summary>
    /// Releases the currently held draggable object.
    /// </summary>
    private void ReleaseGrab()
    {
        if (grabbedObject != null)
        {
            Collider2D boxCollider = grabbedObject.GetComponent<Collider2D>();
            if (playerCollider != null && boxCollider != null)
            {
                // Start a coroutine to re‑enable collisions after a short delay.
                StartCoroutine(ReenableCollisionAfterDelay(playerCollider, boxCollider));
            }
            grabbedObject.Release();
            grabbedObject = null;
        }
    }


    #endregion

    #region Gizmos

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (grabPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }

    #endregion

    // Optional: Expose the facing direction if needed by other scripts.
    public bool IsFacingRight() => isFacingRight;

    private IEnumerator ReenableCollisionAfterDelay(Collider2D playerCol, Collider2D boxCol)
    {
        // Wait until the next physics update.
        yield return new WaitForFixedUpdate();

        // Re‑enable collisions between the player and the box.
        Physics2D.IgnoreCollision(playerCol, boxCol, false);
        Debug.Log("Restored collisions between player and " + boxCol.gameObject.name);
    }

}
