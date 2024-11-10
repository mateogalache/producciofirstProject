using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration;
    public float groundSpeed;
    public float jumpSpeed;
    [Range(0f, 1f)]
    public float groundDecay;
    public Rigidbody2D body;
    public bool grounded;

    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    float xInput;

    
    public float interactionRange = 3f;
    private DraggableObject draggableObject;
    public LayerMask draggableLayer; 

    void Start()
    {
        if (body == null) body = GetComponent<Rigidbody2D>();
        if (groundCheck == null) groundCheck = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        GetInput();
        HandleJump();
        MoveWithInput();
        CheckForDraggable();
        HandleDrag();
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyFriction();
    }

    void GetInput()
    {
        xInput = Input.GetAxis("Horizontal");
    }

    void MoveWithInput()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -groundSpeed, groundSpeed);
            body.velocity = new Vector2(newSpeed, body.velocity.y);
            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        }
    }

    void CheckGround()
    {
        grounded = Physics2D.OverlapBox(groundCheck.bounds.center, groundCheck.bounds.size, 0f, groundMask) != null;
    }

    void ApplyFriction()
    {
        if (grounded && xInput == 0 && body.velocity.y <= 0)
        {
            body.velocity *= groundDecay;
        }
    }

    void CheckForDraggable()
    {
        draggableObject = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, draggableLayer);

        foreach (Collider2D collider in colliders)
        {
            DraggableObject obj = collider.GetComponent<DraggableObject>();
            if (obj != null)
            {
                draggableObject = obj;
                break;
            }
        }
    }

    void HandleDrag()
    {
        if (draggableObject != null && Input.GetKey(KeyCode.LeftShift))
        {
            draggableObject.StartDragging(transform);
        }
        else if (draggableObject != null)
        {
            draggableObject.StopDragging();
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.bounds.center, groundCheck.bounds.size);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange); 
    }
}
