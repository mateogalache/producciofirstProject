using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : PickUpController
{
    //
    protected override void PickUp()
    {
        
        gameObject.SetActive(false);
    }
}
