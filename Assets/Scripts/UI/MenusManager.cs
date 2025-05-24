using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour
{
    public static MenusManager Instance { get; private set; }

    [Header("Roots de UI (arrástralos aquí)")]
    [SerializeField] private GameObject levelUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject abilitiesUI;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button btnReturn;
    [SerializeField] private Button btnSaveAndExit;
    [SerializeField] private Button btnExitToMain;


    private enum MenuState { None, Pause, Controls, Abilities }
    private MenuState currentState = MenuState.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        btnReturn.onClick.AddListener(CloseAll);
        btnSaveAndExit.onClick.AddListener(SaveAndExit);
        btnExitToMain.onClick.AddListener(ExitToMainMenu);
    }

    private void Start()
    {
        if (levelUI == null || pauseUI == null || controlsUI == null || abilitiesUI == null)
        {
            Debug.LogError("[MenusManager] Te falta arrastrar alguna UI Root en el Inspector");
            enabled = false;
            return;
        }
        CloseAll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Toggle(MenuState.Pause);

        if (Input.GetKeyDown(KeyCode.X))
            Toggle(MenuState.Controls);

        if (Input.GetKeyDown(KeyCode.Z))
            Toggle(MenuState.Abilities);
    }

    private void Toggle(MenuState target)
    {
        if (currentState == target)
            CloseAll();
        else
            Open(target);
    }

    private void Open(MenuState target)
    {
        
        levelUI.SetActive(false);
        pauseUI.SetActive(false);
        controlsUI.SetActive(false);
        abilitiesUI.SetActive(false);

        Time.timeScale = 0;
        currentState = target;

        switch (target)
        {
            case MenuState.Pause: if (pauseUI) pauseUI.SetActive(true); break;
            case MenuState.Controls: if (controlsUI) controlsUI.SetActive(true); break;
            case MenuState.Abilities: if (abilitiesUI) abilitiesUI.SetActive(true); break;
        }
    }

    private void CloseAll()
    {
        pauseUI.SetActive(false);
        controlsUI.SetActive(false);
        abilitiesUI.SetActive(false);
        levelUI.SetActive(true);


        if (levelUI) levelUI.SetActive(true);

        Time.timeScale = 1;
        currentState = MenuState.None;
    }

    public void ReturnToGame() => CloseAll();

    public void SaveAndExit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public void ExitToMainMenu() => SaveAndExit();
}