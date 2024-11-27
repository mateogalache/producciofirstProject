using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUpController : MonoBehaviour
{

    protected abstract void PickUp();

    private bool alreadyPicked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyPicked) return;

        PlayerMovement player =  collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            PickUp();
            alreadyPicked = true;
        }        
    }
}
