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

        PlayerController player =  collision.GetComponent<PlayerController>();
        if (player != null)
        {
            PickUp();
            alreadyPicked = true;
        }        
    }
}
