//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Botones del menú")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        continueButton.interactable = GameManager.Instance != null
                                 && GameManager.Instance.HasSavedProgress();

    }

    public void OnNewGameClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartNewGame();
        else
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void OnContinueClicked()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasSavedProgress())
            GameManager.Instance.RestartLevel();
        else
            OnNewGameClicked();
    }

    public void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}