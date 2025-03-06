using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TutorialUI : MonoBehaviour
{
    public RectTransform tutorialPanel;
    public RectTransform cogerObjetosPanel;
    public RectTransform estrellasPanel;
    public RectTransform runPanel;
    public Canvas canvas;

    private Vector2 posicionInicialTutorial;
    private Vector2 posicionInicialCogerObjetos;
    private Vector2 posicionInicialEstrellas;
    private Vector2 posicionInicialRun;

    private float velocidad = 2f;  // velocitat de moviment
    private float amplitud = 10f;  // Ajusta què tant es mou de dalt a baix
    private float DisappearTime = 10f;
    private float estrellasVisibleTime = -1f;
    private float runVisibleTime = -1f;

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
        posicionInicialEstrellas = estrellasPanel.anchoredPosition;
        posicionInicialRun = runPanel.anchoredPosition;

        tutorialPanel.gameObject.SetActive(true); // mostra tutorial al iniciar
        cogerObjetosPanel.gameObject.SetActive(true);
        estrellasPanel.gameObject.SetActive(true);
        runPanel.gameObject.SetActive(true);

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

        /*
        CheckPanelVisibility(estrellasPanel, ref estrellasVisibleTime);

        CheckPanelVisibility(runPanel, ref runVisibleTime);

        //Comparem el temps actual Time.time amb el moment en el panell va ser vist
        if (estrellasVisibleTime > 0 && Time.time - estrellasVisibleTime > DisappearTime)
        {
            estrellasPanel.gameObject.SetActive(false);
            estrellasVisibleTime = -1f;
        }

        if (runVisibleTime > 0 && Time.time - runVisibleTime > DisappearTime)
        {
            runPanel.gameObject.SetActive(false);
            runVisibleTime = -1f;
        }
        */

    }

    /*
     *  void CheckPanelVisibility(RectTransform panel, ref float visibleTime)
    {
        if (panel.gameObject.activeSelf && visibleTime < 0)
        {
            Vector3[] corners = new Vector3[4];
            panel.GetWorldCorners(corners);
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            bool isVisible = false;
            foreach (Vector3 corner in corners)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(canvasRect, corner, Camera.main))
                {
                    isVisible = true;
                    break;
                }
            }

            if (isVisible)
            {
                visibleTime = Time.time;
            }
        }
    }

     
     */


}