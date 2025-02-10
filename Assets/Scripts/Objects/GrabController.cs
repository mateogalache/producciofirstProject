using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform originalParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Called by the player when grabbing this object.
    /// </summary>
    /// <param name="newParent">The transform (usually the player's grab point) to attach to.</param>
    public void Grab(Transform newParent)
    {
        // Save the original parent in case you need to restore it
        originalParent = transform.parent;
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero; // Snap the object to the grab point
        rb.isKinematic = true; // Disable physics while being carried
    }

    /// <summary>
    /// Called by the player when releasing this object.
    /// </summary>
    public void Release()
    {
        transform.SetParent(originalParent);
        rb.isKinematic = false; // Re-enable physics
    }
}
