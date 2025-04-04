using UnityEngine;

public class DashRechargeObject : MonoBehaviour
{
    [SerializeField] private int rechargeAmount = 1;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Animación flotante
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // Rotación
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDash dash = other.GetComponent<PlayerDash>();
            if (dash != null)
            {
                // La recarga ahora se maneja en PlayerDash
                Destroy(gameObject);
            }
        }
    }
}