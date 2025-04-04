using UnityEngine;

public class MoverDerecha : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    [SerializeField] private float velocidadInicial = 5f;
    [SerializeField] private float velocidadMaxima = 15f;
    [SerializeField] private float tiempoAceleracion = 10f;
    [SerializeField] private AnimationCurve curvaAceleracion;

    [Header("Configuración de Checkpoint")]
    [SerializeField] private float distanciaInicial = 3f;

    private Transform playerTransform;
    private Vector3 ultimoCheckpoint;
    private bool debeEsperarJugador = false;
    private float tiempoMovimiento;
    private float velocidadActual;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        ultimoCheckpoint = playerTransform.position;
        velocidadActual = velocidadInicial;

        // Configurar curva de aceleración por defecto si no está asignada
        if (curvaAceleracion.length == 0)
        {
            curvaAceleracion = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }

    void Update()
    {
        if (debeEsperarJugador)
        {
            ReposicionarDetrasDelJugador();
        }
        else
        {
            CalcularVelocidad();
            MoverDerecha2();
        }
    }

    private void ReposicionarDetrasDelJugador()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(
                playerTransform.position.x - distanciaInicial,
                transform.position.y,
                transform.position.z
            );

            if (playerTransform.position.x > transform.position.x + distanciaInicial * 0.5f)
            {
                debeEsperarJugador = false;
                // Reiniciamos el contador de aceleración al reaparecer
                tiempoMovimiento = 0f;
                velocidadActual = velocidadInicial;
            }
        }
    }

    private void CalcularVelocidad()
    {
        if (velocidadActual < velocidadMaxima)
        {
            tiempoMovimiento += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempoMovimiento / tiempoAceleracion);
            float evaluacionCurva = curvaAceleracion.Evaluate(progreso);
            velocidadActual = Mathf.Lerp(velocidadInicial, velocidadMaxima, evaluacionCurva);
        }
    }

    private void MoverDerecha2()
    {
        transform.Translate(Vector3.right * velocidadActual * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ManejarColisionJugador();
        }
        else if (other.CompareTag("Checkpoint"))
        {
            ActualizarCheckpoint(other.transform);
        }
    }

    private void ManejarColisionJugador()
    {
        if (playerTransform != null)
        {
            playerTransform.position = ultimoCheckpoint;
            debeEsperarJugador = true;

            // Opcional: Efectos visuales/sonido
            // Ejemplo: PlaySound(), ParticleSystem.Play(), etc.
        }
    }

    private void ActualizarCheckpoint(Transform checkpoint)
    {
        ultimoCheckpoint = checkpoint.position;
        checkpoint.gameObject.SetActive(false);

        // Opcional: Notificar al GameManager
        // GameManager.Instance?.CheckpointAlcanzado(checkpoint.position);
    }
}