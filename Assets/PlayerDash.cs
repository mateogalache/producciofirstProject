using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftControl;
    [SerializeField] private LayerMask dashObstacleLayers;

    [Header("Límites de Usos")]
    [SerializeField] private int maxDashUses = 5;
    [SerializeField] private string dashRechargeTag = "dashObject";
    [SerializeField] private int rechargeAmount = 1;

    [Header("Dash Aéreo")]
    [SerializeField] private bool allowAirDash = true;
    [SerializeField] private int maxAirDashes = 1;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem dashParticles;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private GameObject dashRechargeEffect;

    [Header("Eventos")]
    public UnityEvent OnDashUsed;
    public UnityEvent OnDashRecharged;
    public UnityEvent OnDashBlocked;

    // Variables privadas
    private Rigidbody2D rb;
    private PlayerMovement movement;
    private Collider2D playerCollider;
    private float lastDashTime;
    private bool isDashing;
    private Vector2 dashDirection;
    private int airDashesRemaining;
    private int currentDashUses;
    private bool dashBlocked;

    // Propiedades públicas
    public int CurrentDashUses => currentDashUses;
    public bool IsDashing => isDashing;
    public bool DashBlocked => dashBlocked;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider2D>();

        InitializeDashSystem();
    }

    private void InitializeDashSystem()
    {
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        airDashesRemaining = maxAirDashes;
        currentDashUses = maxDashUses;
        dashBlocked = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(dashKey))
        {
            TryDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDashMovement();
        }
    }

    private void TryDash()
    {
        if (dashBlocked) return;

        // Verificar condiciones
        bool canDash = currentDashUses > 0 &&
                      Time.time > lastDashTime + dashCooldown &&
                      (movement.IsGrounded() || (allowAirDash && airDashesRemaining > 0));

        if (!canDash)
        {
            OnDashBlocked?.Invoke();
            return;
        }

        StartDash();
    }

    private void StartDash()
    {
        // Consumir recursos
        currentDashUses--;
        if (!movement.IsGrounded())
        {
            airDashesRemaining--;
        }

        // Configurar dash
        CalculateDashDirection();
        isDashing = true;
        lastDashTime = Time.time;

        // Efectos
        PlayDashEffects();

        // Control
        movement.SetMovementControl(false);
        SetInvincibility(true);

        // Evento
        OnDashUsed?.Invoke();

        Invoke("EndDash", dashDuration);
    }

    private void CalculateDashDirection()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        dashDirection = (inputX == 0 && inputY == 0) ?
            (movement.IsFacingRight ? Vector2.right : Vector2.left) :
            new Vector2(inputX, inputY).normalized;
    }

    private void HandleDashMovement()
    {
        rb.velocity = dashDirection * dashSpeed;

        // Opcional: Detección de choque durante dash
        if (Physics2D.Raycast(transform.position, dashDirection, 0.5f, dashObstacleLayers))
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        if (!isDashing) return;

        isDashing = false;
        movement.SetMovementControl(true);

        // Frenado progresivo
        rb.velocity *= 0.5f;

        // Resetear efectos
        if (dashTrail != null) dashTrail.emitting = false;
        SetInvincibility(false);
    }

    private void PlayDashEffects()
    {
        if (dashParticles != null)
        {
            dashParticles.Play();
        }

        if (dashSound != null)
        {
            AudioSource.PlayClipAtPoint(dashSound, transform.position, 0.5f);
        }

        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }
    }

    private void SetInvincibility(bool state)
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = !state;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(dashRechargeTag))
        {
            RechargeDash(other.transform.position);
            Destroy(other.gameObject);
        }
    }

    private void RechargeDash(Vector3 position)
    {
        currentDashUses = Mathf.Min(currentDashUses + rechargeAmount, maxDashUses);

        // Efecto de recarga
        if (dashRechargeEffect != null)
        {
            Instantiate(dashRechargeEffect, position, Quaternion.identity);
        }

        // Evento
        OnDashRecharged?.Invoke();
    }

    public void ResetAirDashes()
    {
        airDashesRemaining = maxAirDashes;
    }

    public void BlockDash(bool block)
    {
        dashBlocked = block;
    }

    public void ResetDashSystem()
    {
        InitializeDashSystem();
    }
}