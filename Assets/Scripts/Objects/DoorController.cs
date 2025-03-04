using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen = false;

    public void OpenDoor()
    {
        if (!isOpen)
        {
            Debug.Log("¡Puerta abierta!");
            // Ejemplo sencillo: desactivar el objeto para abrir la puerta.
            gameObject.SetActive(false);
            isOpen = true;
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            Debug.Log("¡Puerta cerrada!");
            // Vuelve a activar el objeto para cerrar la puerta.
            gameObject.SetActive(true);
            isOpen = false;
        }
    }
}
