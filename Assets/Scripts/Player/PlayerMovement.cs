using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAudio))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationTime = 0.1f;
    [SerializeField] private float decelerationTime = 0.05f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Carry Settings")]
    [SerializeField] private float jumpCarryMultiplier = 0.8f;

    [Header("Drag Settings")]
    [SerializeField] private Transform grabPointR;
    [SerializeField] private Transform grabPointL;
    [SerializeField] private float grabRadius = 1f;
    [SerializeField] private LayerMask draggableLayer;

    [Header("Glide Settings")]
    [SerializeField] private float glideGravityScale = 0.5f;
    [SerializeField] private float maxGlideFallSpeed = -2f;
    private bool isGliding = false;

    [Header("Dash Settings")]
    [SerializeField] private PlayerDash dashComponent;

    private Rigidbody2D rb;
    private PlayerAudio playerAudio;
    private Animator animator;
    private Collider2D playerCollider;

    private float horizontalInput;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float velocityXSmoothing;

    private bool isFacingRight = true;
    private bool canDoubleJump;
    private bool enabledMovement = true;

    private DraggableObject grabbedObject;
    public SpriteRenderer body;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        playerAudio = GetComponent<PlayerAudio>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!enabledMovement) return;

        GetInput();
        HandleJumpBuffer();
        UpdateCoyoteTimer();
        HandleGrabInput();
        HandleFlip();
        UpdateAnimations();
        HandleGlideInput();
    }

    void FixedUpdate()
    {
        if (!enabledMovement) return;

        Move();
        ApplyCustomGravity();
    }

    #region Movement Core
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

    private void UpdateCoyoteTimer()
    {
        if (IsGrounded())
        {
            coyoteTimer = coyoteTime;
            canDoubleJump = true;
            if (dashComponent != null)
            {
                dashComponent.ResetAirDashes();
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void Move()
    {
        float currentSpeed = (grabbedObject != null) ? moveSpeed * 0.7f : moveSpeed;
        float targetSpeed = horizontalInput * currentSpeed;
        float smoothTime = Mathf.Abs(horizontalInput) > 0.01f ? accelerationTime : decelerationTime;
        float newVelocityX = Mathf.SmoothDamp(rb.velocity.x, targetSpeed, ref velocityXSmoothing, smoothTime);
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
    }
    #endregion

    #region Jump System
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

    private void Jump()
    {
        float finalJumpForce = jumpForce;

        if (grabbedObject != null)
        {
            finalJumpForce = jumpForce * jumpCarryMultiplier;
        }

        rb.velocity = new Vector2(rb.velocity.x, finalJumpForce);
        playerAudio?.PlayJumpSound();
    }
    #endregion

    #region Physics
    private void ApplyCustomGravity()
    {
        if (isGliding)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (glideGravityScale - 1) * Time.fixedDeltaTime;
            if (rb.velocity.y < maxGlideFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxGlideFallSpeed);
            }
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityScale * 2 - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
            }
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    #endregion

    #region Visuals
    private void HandleFlip()
    {
        if ((horizontalInput > 0 && !isFacingRight) ||
            (horizontalInput < 0 && isFacingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        //Vector3 scale = transform.localScale;
        //scale.x *= -1;
        body.flipX = !body.flipX;
        //transform.localScale = scale;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
        animator.SetBool("IsCarrying", grabbedObject != null);
        animator.SetBool("IsGliding", isGliding);
    }
    #endregion

    #region Special Abilities
    private void HandleGlideInput()
    {
        if (!IsGrounded() && rb.velocity.y < 0 && Input.GetButton("Jump"))
        {
            isGliding = true;
        }
        else
        {
            isGliding = false;
        }
    }

    public void SetMovementControl(bool enabled)
    {
        enabledMovement = enabled;
        if (!enabled)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    #endregion

    #region Grab System
    private void HandleGrabInput()
    {
        if (!enabledMovement) return;

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

        Transform grabPoint = IsFacingRight ? grabPointR : grabPointL;

        Collider2D[] hits = Physics2D.OverlapCircleAll(grabPoint.position, grabRadius, draggableLayer);
        if (hits.Length == 0) return;

        foreach (Collider2D col in hits)
        {
            DraggableObject dObj = col.GetComponentInParent<DraggableObject>();
            if (dObj != null)
            {
                grabbedObject = dObj;
                grabbedObject.Grab(grabPoint);
                //Physics2D.IgnoreCollision(playerCollider, col, true);
                return;
            }
        }
    }

    private void ReleaseGrab()
    {
        if (grabbedObject != null)
        {
            Collider2D boxCollider = grabbedObject.GetComponent<Collider2D>();
            if (playerCollider != null && boxCollider != null)
            {
                //StartCoroutine(ReenableCollisionAfterDelay(playerCollider, boxCollider));
            }
            grabbedObject.Release();
            grabbedObject = null;
        }
    }

    private IEnumerator ReenableCollisionAfterDelay(Collider2D playerCol, Collider2D boxCol)
    {
        yield return new WaitForFixedUpdate();
        Physics2D.IgnoreCollision(playerCol, boxCol, false);
    }
    #endregion

    #region Public Accessors
    public bool IsFacingRight
    {
        get { return isFacingRight; }
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
        if (grabPointR != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(grabPointR.position, grabRadius);
        }
    }
    #endregion
}