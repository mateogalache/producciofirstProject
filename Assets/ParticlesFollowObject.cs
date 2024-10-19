using UnityEngine;

public class ParticlesFollowObject : MonoBehaviour
{
    // Referencia al GameObject que quieres que sigan las partículas
    public Transform target;

    // Velocidad con la que las partículas seguirán el objeto
    public float followSpeed = 5f;

    // Offset opcional para ajustar la posición
    public Vector3 offset;

    private ParticleSystem particleSystem;

    void Start()
    {
        // Obtén el componente de partículas si está en el mismo objeto
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (target != null)
        {
            // Calcula la nueva posición de las partículas
            Vector3 targetPosition = target.position + offset;

            // Mueve el sistema de partículas hacia el GameObject objetivo
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
