using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAudio))] // Aseg�rate de tener un script PlayerAudio (o elim�nalo si no lo usas)
public class PlayerMovementLvl2 : MonoBehaviour
{
    #region Movement Settings
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationTime = 0.1f;   // Tiempo para alcanzar la velocidad completa al moverse
    [SerializeField] private float decelerationTime = 0.05f;    // Tiempo para desacelerar al dejar de moverse
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer; // Capas consideradas como suelo
    #endregion

    #region Jump & Buffer Settings
    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;       // Permite saltar poco despu�s de salir del suelo
    [SerializeField] private float jumpBufferTime = 0.2f;   // Buffer para el salto antes de aterrizar
    #endregion

    #region Carry Settings
    [Header("Carry Settings")]
    [SerializeField] private float jumpCarryMultiplier = 0.8f; // Reduce la fuerza del salto al cargar un objeto
    #endregion

    #region Drag Settings
    [Header("Drag Settings")]
    [SerializeField] private Transform grabPoint;         // Punto de agarre del jugador
    [SerializeField] private float grabRadius = 1f;         // Radio de detecci�n para objetos movibles
    [SerializeField] private LayerMask draggableLayer;      // Capa asignada a objetos movibles
    #endregion

    #region Glide Settings
    [Header("Glide Settings")]
    [SerializeField] private float glideGravityScale = 0.5f; // Factor para reducir la gravedad al planear
    [SerializeField] private float maxGlideFallSpeed = -2f;  // Velocidad m�xima de ca�da cuando se planea
    [SerializeField] private float glideSpeedMultiplier = 1.5f; // Multiplicador para aumentar la velocidad horizontal durante el planeo
    private bool isGliding = false;
    #endregion

    #region Wall & Wall Jump Settings
    [Header("Wall Settings")]
    [SerializeField] private float wallCheckDistance = 0.5f; // Distancia desde el jugador para detectar la pared
    [SerializeField] private float wallCheckRadius = 0.2f;   // Radio de detecci�n de la pared
    [SerializeField] private LayerMask wallLayer;            // Capa asignada a las paredes

    [Header("Wall Jump Settings")]
    [SerializeField] private Vector2 wallJumpForce = new Vector2(10f, 20f); // Fuerza aplicada al wall jump (x: horizontal, y: vertical)
    #endregion

    // Componentes y variables internas
    private Rigidbody2D rb;
    private PlayerAudio playerAudio;
    private Animator animator; // Opcional � si usas animaciones

    private float horizontalInput;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float velocityXSmoothing;

    private bool isFacingRight = true;
    private bool canDoubleJump;

    // Referencia al objeto movible actualmente agarrado (si existe)
    private DraggableObject grabbedObject;
    private Collider2D playerCollider;
    // Permite que AdvancedCameraFollow consulte si estamos planeando
    public bool IsGliding => isGliding;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        playerAudio = GetComponent<PlayerAudio>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>(); // Si usas animaciones
    }

    void Update()
    {
        GetInput();
        HandleJumpBuffer();
        UpdateCoyoteTimer();
        HandleGrabInput();
        HandleFlip();
        UpdateAnimations();
        HandleGlideInput(); // Comprueba si se activa el planeo
    }

    void FixedUpdate()
    {
        Move();
        ApplyCustomGravity();
    }

    #region Input & Movement
    /// <summary>
    /// Lee el input horizontal y el de salto.
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
    /// Actualiza el temporizador "coyote" para permitir saltos poco despu�s de salir del suelo.
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
    /// Verifica si hay un salto en buffer y si se cumplen las condiciones para saltar.
    /// Se incluye el wall jump si el jugador est� en el aire y toca una pared.
    /// </summary>
    private void HandleJumpBuffer()
    {
        if (jumpBufferTimer > 0f)
        {
            if (IsGrounded() || (!IsGrounded() && IsTouchingWall()) || canDoubleJump)
            {
                Jump();
                jumpBufferTimer = 0f;
                if (!IsGrounded() && !IsTouchingWall() && canDoubleJump)
                    canDoubleJump = false;
            }
        }
    }

    /// <summary>
    /// Ejecuta el salto. Si el jugador est� en el aire y tocando una pared, se realiza un wall jump.
    /// </summary>
    private void Jump()
    {
        // Si no est� en el suelo y est� tocando una pared, ejecuta el wall jump
        if (!IsGrounded() && IsTouchingWall())
        {
            int wallDir = WallDirection();
            // Aplica la fuerza del wall jump en direcci�n contraria a la pared
            rb.velocity = new Vector2(-wallDir * wallJumpForce.x, wallJumpForce.y);
            // Opcional: invierte la direcci�n del sprite para reflejar el salto
            isFacingRight = (wallDir < 0);
        }
        else
        {
            // Salto normal
            float finalJumpForce = jumpForce;
            if (grabbedObject != null)
            {
                finalJumpForce = jumpForce * jumpCarryMultiplier;
            }
            rb.velocity = new Vector2(rb.velocity.x, finalJumpForce);
        }
        playerAudio?.PlayJumpSound();
    }

    /// <summary>
    /// Maneja el movimiento horizontal, aplicando un multiplicador extra si se est� planeando.
    /// </summary>
    private void Move()
    {
        float baseSpeed = (grabbedObject != null) ? moveSpeed * 0.7f : moveSpeed;
        if (isGliding)
        {
            baseSpeed *= glideSpeedMultiplier;
        }
        float targetSpeed = horizontalInput * baseSpeed;
        float smoothTime = Mathf.Abs(horizontalInput) > 0.01f ? accelerationTime : decelerationTime;
        float newVelocityX = Mathf.SmoothDamp(rb.velocity.x, targetSpeed, ref velocityXSmoothing, smoothTime);
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
    }

    /// <summary>
    /// Aplica la gravedad personalizada y ajusta la f�sica durante el planeo.
    /// </summary>
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

    /// <summary>
    /// Comprueba si el jugador est� en el suelo.
    /// </summary>
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Invierte la direcci�n del sprite del jugador seg�n el movimiento.
    /// </summary>
    private void HandleFlip()
    {
        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Actualiza los par�metros del Animator (si se utiliza) para reflejar el estado actual.
    /// </summary>
    private void UpdateAnimations()
    {
        Debug.Log(rb.velocity.y);
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsGrounded", IsGrounded());
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
            animator.SetFloat("HorizontalVelocity", rb.velocity.x);
            animator.SetBool("IsCarrying", grabbedObject != null);
            animator.SetBool("IsGliding", isGliding);
            animator.SetBool("IsDashing", false); // Cambia esto si tienes un dash
        }
    }

    
    /// <summary>
    /// Controla la activaci�n del planeo: se activa si el jugador est� en el aire, cayendo y mantiene pulsado salto.
    /// </summary>
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
    #endregion

    #region Wall Jump Helpers
    /// <summary>
    /// Detecta si el jugador est� tocando una pared a la izquierda o a la derecha.
    /// </summary>
    private bool IsTouchingWall()
    {
        Vector2 position = transform.position;
        bool touchingLeft = Physics2D.OverlapCircle(position + Vector2.left * wallCheckDistance, wallCheckRadius, wallLayer);
        bool touchingRight = Physics2D.OverlapCircle(position + Vector2.right * wallCheckDistance, wallCheckRadius, wallLayer);
        return touchingLeft || touchingRight;
    }

    /// <summary>
    /// Determina en qu� direcci�n est� la pared: -1 si est� a la izquierda, 1 si a la derecha.
    /// </summary>
    private int WallDirection()
    {
        Vector2 position = transform.position;
        bool touchingLeft = Physics2D.OverlapCircle(position + Vector2.left * wallCheckDistance, wallCheckRadius, wallLayer);
        bool touchingRight = Physics2D.OverlapCircle(position + Vector2.right * wallCheckDistance, wallCheckRadius, wallLayer);
        if (touchingLeft) return -1;
        if (touchingRight) return 1;
        return 0;
    }
    #endregion

    #region Dragging
    /// <summary>
    /// Maneja el input para agarrar o soltar un objeto movible.
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

    /// <summary>
    /// Intenta agarrar un objeto dentro del �rea definida.
    /// </summary>
    private void TryGrab()
    {
        if (grabPoint == null)
        {
            Debug.LogWarning("GrabPoint no asignado en el jugador.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(grabPoint.position, grabRadius, draggableLayer);
        if (hits.Length == 0)
        {
            return;
        }

        foreach (Collider2D col in hits)
        {
            DraggableObject dObj = col.GetComponentInParent<DraggableObject>();
            if (dObj != null)
            {
                grabbedObject = dObj;
                grabbedObject.Grab(grabPoint);
                Collider2D boxCollider = grabbedObject.GetComponent<Collider2D>();
                if (playerCollider != null && boxCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, boxCollider, true);
                }
                return;
            }
        }
    }

    /// <summary>
    /// Suelta el objeto movible actualmente agarrado.
    /// </summary>
    private void ReleaseGrab()
    {
        if (grabbedObject != null)
        {
            Collider2D boxCollider = grabbedObject.GetComponent<Collider2D>();
            if (playerCollider != null && boxCollider != null)
            {
                StartCoroutine(ReenableCollisionAfterDelay(playerCollider, boxCollider));
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

    #region Gizmos
    /// <summary>
    /// Dibuja gizmos para visualizar las �reas de detecci�n del suelo, paredes y agarre.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        // Dibuja los c�rculos para la detecci�n de pared (izquierda y derecha)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.left * wallCheckDistance, wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + Vector3.right * wallCheckDistance, wallCheckRadius);

        if (grabPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }
    #endregion
}
