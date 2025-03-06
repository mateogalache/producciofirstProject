using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TutorialUI : MonoBehaviour
{
    public RectTransform tutorialPanel;
    public RectTransform cogerObjetosPanel;

    private Vector2 posicionInicialTutorial;
    private Vector2 posicionInicialCogerObjetos;
    private float velocidad = 2f;  // velocitat de moviment
    private float amplitud = 10f;  // Ajusta què tant es mou de dalt a baix

    void Start()
    {
        if (tutorialPanel == null || cogerObjetosPanel == null)
        {
            Debug.LogError("TutorialUI: Paneles no asignados en el Inspector.");
            return;
        }

        //Guardem posicio inical de cada panell
        posicionInicialTutorial = tutorialPanel.anchoredPosition;
        posicionInicialCogerObjetos = cogerObjetosPanel.anchoredPosition;

        tutorialPanel.gameObject.SetActive(true); // mostra tutorial al iniciar
        cogerObjetosPanel.gameObject.SetActive(true);

    }

    void Update()
    {

        if (tutorialPanel == null || cogerObjetosPanel == null) return;

        float moviment = Mathf.Sin(Time.time * velocidad) * amplitud;

        //Movem panels
        Vector2 vector2 = new(posicionInicialTutorial.x, posicionInicialTutorial.y + moviment);
        tutorialPanel.anchoredPosition = vector2;
        cogerObjetosPanel.anchoredPosition = new Vector2(posicionInicialCogerObjetos.x, posicionInicialCogerObjetos.y + moviment);

        if (Input.GetKeyDown(KeyCode.Space)) // Si pressionem espacio
        {
            tutorialPanel.gameObject.SetActive(false); // Ocultem tutorial
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            cogerObjetosPanel.gameObject.SetActive(false);
        }

    }
}