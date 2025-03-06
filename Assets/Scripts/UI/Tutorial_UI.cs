using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    public RectTransform tutorialPanel;
    public RectTransform cogerObjetosPanel;
    public RectTransform estrellasPanel;
    public RectTransform runPanel;

    public GameObject checkpointPanel;
    public TMP_Text checkpointText;

    private Vector2 posicionInicialTutorial;
    private Vector2 posicionInicialCogerObjetos;
    private Vector2 posicionInicialEstrellas;
    private Vector2 posicionInicialRun;

    private float velocidad = 2f;  // velocitat de moviment
    private float amplitud = 10f;  // Ajusta què tant es mou de dalt a baix
    

    void Start()
    {
        if (tutorialPanel == null || cogerObjetosPanel == null || checkpointPanel == null || checkpointText == null)
        {
            Debug.LogError("TutorialUI: Paneles no asignados en el Inspector.");
            return;
        }

        //Guardem posicio inical de cada panell
        posicionInicialTutorial = tutorialPanel.anchoredPosition;
        posicionInicialCogerObjetos = cogerObjetosPanel.anchoredPosition;
        posicionInicialEstrellas = estrellasPanel.anchoredPosition;
        posicionInicialRun = runPanel.anchoredPosition;

        tutorialPanel.gameObject.SetActive(true); // mostra tutorial al iniciar
        cogerObjetosPanel.gameObject.SetActive(true);
        estrellasPanel.gameObject.SetActive(true);
        runPanel.gameObject.SetActive(true);
        checkpointPanel.SetActive(false);

    }

    void Update()
    {

        if (tutorialPanel == null || cogerObjetosPanel == null || estrellasPanel == null || runPanel == null) return;

        float moviment = Mathf.Sin(Time.time * velocidad) * amplitud;

        //Movem panels
        tutorialPanel.anchoredPosition = new Vector2(posicionInicialTutorial.x, posicionInicialTutorial.y + moviment);
        cogerObjetosPanel.anchoredPosition = new Vector2(posicionInicialCogerObjetos.x, posicionInicialCogerObjetos.y + moviment);
        estrellasPanel.anchoredPosition = new Vector2(posicionInicialEstrellas.x, posicionInicialEstrellas.y + moviment);
        runPanel.anchoredPosition = new Vector2(posicionInicialRun.x, posicionInicialRun.y + moviment);


        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            tutorialPanel.gameObject.SetActive(false); // Ocultem tutorial
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            cogerObjetosPanel.gameObject.SetActive(false);
        }

    }

    public void ShowMessage(string message)
    {
        if (checkpointText != null && checkpointPanel != null)
        {
            checkpointText.text = message;
            checkpointPanel.SetActive(true);
            Debug.Log("Mensaje mostrado en TutorialUI: " + message);
        } else
        {
            Debug.LogError("checkpointText o checkpointPanel no están asignados en el Inspector.");
        }
    }

}