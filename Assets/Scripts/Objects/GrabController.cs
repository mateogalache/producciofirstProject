using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DraggableObject : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform targetPoint; 
    private bool isBeingCarried = false;

    [Header("Grab settings")]
    [SerializeField] private float followSpeed = 10f;     

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isBeingCarried && targetPoint != null)
        {
            Vector2 newPos = Vector2.Lerp(rb.position, targetPoint.position, followSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    /// <summary>
    /// Inicia el agarre del objeto.
    /// </summary>
    public void Grab(Transform followTarget)
    {
        targetPoint = followTarget;
        isBeingCarried = true;
    }

    /// <summary>
    /// Suelta el objeto.
    /// </summary>
    public void Release()
    {
        isBeingCarried = false;
        targetPoint = null;
    }
}
