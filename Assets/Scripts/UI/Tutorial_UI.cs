using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TutorialUI : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject grabObjectsPanel;

    void Start()
    {
        tutorialPanel.SetActive(true); // mostra tutorial al iniciar
        grabObjectsPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Si pressionem espacio
        {
            tutorialPanel.SetActive(false); // Ocultem tutorial
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            grabObjectsPanel.SetActive(false);
        }

    }
}