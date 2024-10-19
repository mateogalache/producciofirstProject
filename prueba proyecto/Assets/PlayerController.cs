using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Player movement")]
    public float moveVelocity = 2f;
    public float upVelocity = 3f;
    public Rigidbody2D _rb;
    public GameObject floorDetector;
    public bool isGrounded;
    void Start()
    {
        _rb.GetComponent<Rigidbody2D>();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(floorDetector.transform.position, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(floorDetector.transform.position,0.05f,1<<6))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }




        float horizontalMovement = Input.GetAxis("Horizontal");

        Vector2 velocity = _rb.velocity;
        velocity.x = horizontalMovement * moveVelocity;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = upVelocity;
            //_rb.velocity = Vector3.up * upVelocity;
            //transform.position += addVelocity * new Vector3(0, 1, 0) * Time.deltaTime;
        }

        _rb.velocity = velocity;
        
    }
}
