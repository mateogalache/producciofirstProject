using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockControllerLvl2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisi�n detectada con: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisi�n con Player detectada");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
            gameObject.SetActive(false);
            RockSpawner spawner = FindObjectOfType<RockSpawner>();
            if (spawner != null)
            {
                spawner.ResetSpawner();
            }
        }
    }
}
