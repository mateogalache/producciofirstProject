using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAudio))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer; // Ensure this includes both "Ground" and "Draggable"

    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private float horizontalInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isFacingRight = true;
    private bool canDoubleJump;

    private Rigidbody2D rb;
    private PlayerAudio playerAudio;
    private Animator animator;

    // Reference to the grabbed object
    private DraggableObject grabbedObject;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        playerAudio = GetComponent<PlayerAudio>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleCoyoteTime();
        HandleJumpBuffer();
        FlipCharacter();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        Move();
        ApplyCustomGravity();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandleCoyoteTime()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = true; // Reset double jump when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpBuffer()
    {
        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f || canDoubleJump)
            {
                Jump();
                jumpBufferCounter = 0f;

                if (canDoubleJump && !IsGrounded())
                {
                    canDoubleJump = false;
                }
            }
        }
    }

    private void Jump()
    {
        float finalJumpForce = jumpForce;

        if (grabbedObject != null)
        {
            // Optionally adjust jump force based on box weight
            Rigidbody2D boxRb = grabbedObject.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                // Example: Reduce jump force based on the mass of the box
                finalJumpForce = jumpForce / (1 + boxRb.mass);
            }
        }

        rb.velocity = new Vector2(rb.velocity.x, finalJumpForce);
        playerAudio?.PlayJumpSound();
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    private void ApplyCustomGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityScale * 2) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FlipCharacter()
    {
        if (horizontalInput > 0 && !isFacingRight || horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

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

    // Public methods to set and unset the grabbed object
    public void SetGrabbedObject(DraggableObject obj)
    {
        grabbedObject = obj;
    }

    public void UnsetGrabbedObject()
    {
        grabbedObject = null;
    }

    // Optional: Visualize ground check radius in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Public method to check facing direction
    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}
