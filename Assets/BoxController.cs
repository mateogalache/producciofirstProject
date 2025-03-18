using UnityEngine;

public class BoxController : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Start()
    {
        // Guardamos la posici�n inicial al iniciar la escena
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        // Restablecemos la posici�n inicial de la caja
        transform.position = initialPosition;
    }
}
