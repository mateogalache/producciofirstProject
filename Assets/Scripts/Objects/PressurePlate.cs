using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    // Lista para llevar la cuenta de los objetos v�lidos (cajas y jugador) sobre la placa.
    private List<GameObject> objectsOnPlate = new List<GameObject>();

    // La placa se considerar� activada si hay al menos un objeto en la lista.
    public bool isActivated
    {
        get { return objectsOnPlate.Count > 0; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el objeto es el jugador o una caja, lo a�adimos a la lista (si a�n no est�)
        if ((other.CompareTag("Player") || other.CompareTag("Box")) && !objectsOnPlate.Contains(other.gameObject))
        {
            objectsOnPlate.Add(other.gameObject);
            Debug.Log(gameObject.name + " activada (entr�: " + other.gameObject.name + ")");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Si el objeto es el jugador o una caja, lo removemos de la lista.
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (objectsOnPlate.Contains(other.gameObject))
            {
                objectsOnPlate.Remove(other.gameObject);
                Debug.Log(gameObject.name + " desactivada (sali�: " + other.gameObject.name + ")");
            }
        }
    }
}
