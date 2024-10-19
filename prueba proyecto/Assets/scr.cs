using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour


{
    public float addVelocity = 3f;
    public Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb.GetComponent<Rigidbody>();
    }

    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _rb.velocity = Vector3.up * addVelocity;
            //transform.position += addVelocity * new Vector3(0, 1, 0) * Time.deltaTime;
        }
    }
}
