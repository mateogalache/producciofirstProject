using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging;
    private Transform player;

    public float dragSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D attached to " + gameObject.name);
        }
        else
        {
            rb.isKinematic = true; // Configura inicialmente el Rigidbody2D como kinematic
        }
    }

    public void StartDragging(Transform playerTransform)
    {
        if (rb != null) // Verifica que rb esté asignado
        {
            isDragging = true;
            player = playerTransform;
            rb.isKinematic = false; // Cambia a dynamic cuando empieza a arrastrar
        }
    }

    public void StopDragging()
    {
        isDragging = false;
        player = null;
        if (rb != null)
        {
            rb.isKinematic = true; // Vuelve a kinematic cuando deja de arrastrar
        }
    }

    void FixedUpdate()
    {
        if (isDragging && player != null && rb != null)
        {
            Vector2 targetPosition = new Vector2(player.position.x, rb.position.y);
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, dragSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }
}
