using UnityEngine;

public class MovimientoLateral : MonoBehaviour
{
    public float velocidad = 3f; // Velocidad de movimiento
    private int direccion = 1; // Comienza moviéndose a la derecha

    void Update()
    {
        // Mueve el objeto en la dirección actual
        transform.position += Vector3.right * direccion * velocidad * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto con el que colisiona NO está en la capa "Player"
        if (other.gameObject.layer == LayerMask.NameToLayer("PlatformDetector"))
        {
            direccion *= -1;
        }
    }
}
