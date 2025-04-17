using UnityEngine;
using System.Collections; // Añade esta línea para usar IEnumerator

public class MovimientoLateralLvl2 : MonoBehaviour
{
    public float velocidadMinima = 2f;
    public float velocidadMaxima = 5f;
    private float velocidad;
    private int direccion;
    private float tiempoParaCambioDireccion;
    private float tiempoUltimoCambio;
    private bool puedeCambiarDireccion = true;

    void Start()
    {
        direccion = Random.Range(0, 2) * 2 - 1;
        velocidad = Random.Range(velocidadMinima, velocidadMaxima);
        ConfigurarProximoCambio();
    }

    void Update()
    {
        transform.position += Vector3.right * direccion * velocidad * Time.deltaTime;

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
        
        // Cambio inmediato de dirección
        direccion *= -1;
        velocidad = Random.Range(velocidadMinima, velocidadMaxima);
        ConfigurarProximoCambio();
        
        // Pequeño delay para evitar múltiples triggers
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