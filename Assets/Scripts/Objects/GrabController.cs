using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [SerializeField] private Transform grabPoint;
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDistance;
    private GameObject grabbedObject;
    private int layerIndex;

    private void Start()
    {
        layerIndex = LayerMask.NameToLayer("Draggable");
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex && grabbedObject == null)
            {
                // Grab the object
                grabbedObject = hitInfo.collider.gameObject;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.SetParent(transform); // Attach the box to the player
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (grabbedObject != null)
            {
                // Release the object
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                grabbedObject.transform.SetParent(null); // Detach the box from the player
                grabbedObject = null;
            }
        }

        Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.red);
    }
}
