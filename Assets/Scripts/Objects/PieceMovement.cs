using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    // Variables para controlar el movimiento de flotación
    public float amplitude = 0.5f; // Altura de la flotación
    public float frequency = 1f;   // Frecuencia de la flotación

    // Posición inicial del objeto
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Guardamos la posición inicial
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculamos la nueva posición
        Vector3 tempPosition = startPosition;
        tempPosition.y += Mathf.Sin(Time.time * frequency) * amplitude;

        // Actualizamos la posición del objeto
        transform.position = tempPosition;
    }
}
