using UnityEngine;

public class PressurePlateManager : MonoBehaviour
{
    [Tooltip("Lista de las 4 plataformas de presi�n")]
    public PressurePlate[] pressurePlates;

    [Tooltip("Referencia a la puerta que se abrir� al activar todos los interruptores")]
    public DoorController door;  // Aseg�rate de tener el script DoorController en la puerta.

    void Update()
    {
        bool allActivated = true;
        foreach (PressurePlate plate in pressurePlates)
        {
            if (!plate.isActivated)
            {
                allActivated = false;
                break;
            }
        }
        if (allActivated)
        {
            door.OpenDoor();
        }
        else
        {
            door.CloseDoor();
        }
    }
}
