using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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

    private float velocidad = 2f; 
    private float amplitud = 10f; 

    private RectTransform canvasRect;

    void Start()
    {
        if (tutorialPanel == null || cogerObjetosPanel == null || checkpointPanel == null || checkpointText == null)
        {
            Debug.LogError("TutorialUI: Paneles no asignados en el Inspector.");
            return;
        }

        posicionInicialTutorial = tutorialPanel.anchoredPosition;
        posicionInicialCogerObjetos = cogerObjetosPanel.anchoredPosition;
        posicionInicialEstrellas = estrellasPanel.anchoredPosition;
        posicionInicialRun = runPanel.anchoredPosition;

        tutorialPanel.gameObject.SetActive(true); 
        cogerObjetosPanel.gameObject.SetActive(true);
        estrellasPanel.gameObject.SetActive(true);
        runPanel.gameObject.SetActive(true);
        checkpointPanel.SetActive(false);

        canvasRect = checkpointPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

    }

    void Update()
    {

        if (tutorialPanel == null || cogerObjetosPanel == null || estrellasPanel == null || runPanel == null) return;


        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            tutorialPanel.gameObject.SetActive(false); // Ocultem tutorial
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            cogerObjetosPanel.gameObject.SetActive(false);
        }

        if (Camera.main != null && checkpointPanel.activeSelf)
        {
            UpdateCheckpointPanelPosition();
        }

    }

    private void UpdateCheckpointPanelPosition()
    {
        if (Camera.main == null || checkpointPanel == null || canvasRect == null) return;

        Vector3 screenTopRight = Camera.main.ViewportToScreenPoint(new Vector3(1, 1, 0));

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenTopRight, Camera.main, out localPoint);

        checkpointPanel.GetComponent<RectTransform>().anchoredPosition = localPoint + new Vector2(-40, -40);
    }


    public void ShowMessage(string message)
    {
        if (checkpointText != null && checkpointPanel != null)
        {
            checkpointText.text = message;

            UpdateCheckpointPanelPosition();

            checkpointPanel.SetActive(true);
            Debug.Log("Mensaje mostrado en TutorialUI: " + message);

            Invoke(nameof(HideCheckpointPanel), 3f);

        } else
        {
            Debug.LogError("checkpointText o checkpointPanel no est�n asignados en el Inspector.");
        }
    }

    private void HideCheckpointPanel()
    {
        checkpointPanel.SetActive(false);
    }

}