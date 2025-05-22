using UnityEngine;
using System.Collections; // Necesario para usar IEnumerator

public class MovimientoLateralLvl2 : MonoBehaviour
{
    public float velocidadMinima = 2f;
    public float velocidadMaxima = 5f;
    private float velocidad;
    private int direccion;
    private float tiempoParaCambioDireccion;
    private float tiempoUltimoCambio;
    private bool puedeCambiarDireccion = true;

    // Límites de posición en X donde cambia de dirección automáticamente
    public float limiteDerecho = 875.1f;
    public float limiteIzquierdo = 592.8f;

    void Start()
    {
        direccion = Random.Range(0, 2) * 2 - 1; // Random -1 o 1
        velocidad = Random.Range(velocidadMinima, velocidadMaxima);
        ConfigurarProximoCambio();
    }

    void Update()
    {
        // Movimiento lateral
        transform.position += Vector3.right * direccion * velocidad * Time.deltaTime;

        // Cambio automático si llega a los extremos definidos
        if ((transform.position.x >= limiteDerecho && direccion > 0) ||
            (transform.position.x <= limiteIzquierdo && direccion < 0))
        {
            CambiarDireccion();
        }

        // Cambio por tiempo
        if (Time.time - tiempoUltimoCambio >= tiempoParaCambioDireccion)
        {
            CambiarDireccion();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlatformDetector") && puedeCambiarDireccion)
        {
            StartCoroutine(CambiarDireccionConDelay());
        }
    }

    IEnumerator CambiarDireccionConDelay()
    {
        puedeCambiarDireccion = false;

        // Cambio inmediato de dirección al detectar colisión
        direccion *= -1;
        velocidad = Random.Range(velocidadMinima, velocidadMaxima);
        ConfigurarProximoCambio();

        // Pequeño delay para evitar múltiples cambios rápidos
        yield return new WaitForSeconds(0.2f);
        puedeCambiarDireccion = true;
    }

    void CambiarDireccion()
    {
        direccion *= -1;
        velocidad = Random.Range(velocidadMinima, velocidadMaxima);
        ConfigurarProximoCambio();
    }

    void ConfigurarProximoCambio()
    {
        tiempoParaCambioDireccion = Random.Range(2f, 5f);
        tiempoUltimoCambio = Time.time;
    }
}
