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
        // Singleton
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
        // Al arrancar, asegúrate de que levelUI, pauseUI, ... estén asignados
        if (levelUI == null || pauseUI == null || controlsUI == null || abilitiesUI == null)
        {
            Debug.LogError("[MenusManager] Te falta arrastrar alguna UI Root en el Inspector");
            enabled = false;
            return;
        }
        CloseAll();
    }

    /*
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // Si volvemos al MainMenu, nos auto-destruimos
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
            return;
        }

        // 1) Buscamos en la jerarquía los roots de UI
        levelUI = GameObject.Find("LevelUI");
        pauseUI = GameObject.Find("PauseUI");
        controlsUI = GameObject.Find("ControlsUI");
        abilitiesUI = GameObject.Find("AbilitiesUI");

        // 2) Ocultamos todo y dejamos solo la UI de juego
        levelUI?.SetActive(true);
        pauseUI?.SetActive(false);
        controlsUI?.SetActive(false);
        abilitiesUI?.SetActive(false);
        currentState = MenuState.None;
        Time.timeScale = 1;

        // 3) Volvemos a conectar los botones
        //    (btnReturn, btnSaveAndExit, btnExitToMain están
        //     arrastrados en el Inspector una sola vez)
        btnReturn.onClick.RemoveAllListeners();
        btnReturn.onClick.AddListener(CloseAll);

        btnSaveAndExit.onClick.RemoveAllListeners();
        btnSaveAndExit.onClick.AddListener(SaveAndExit);

        btnExitToMain.onClick.RemoveAllListeners();
        btnExitToMain.onClick.AddListener(ExitToMainMenu);
    }*/

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
        // primero oculta todo
        levelUI.SetActive(false);
        pauseUI.SetActive(false);
        controlsUI.SetActive(false);
        abilitiesUI.SetActive(false);

        // Pausamos el juego
        Time.timeScale = 0;
        currentState = target;

        // Mostramos SOLO el que toque
        switch (target)
        {
            case MenuState.Pause: if (pauseUI) pauseUI.SetActive(true); break;
            case MenuState.Controls: if (controlsUI) controlsUI.SetActive(true); break;
            case MenuState.Abilities: if (abilitiesUI) abilitiesUI.SetActive(true); break;
        }
    }

    private void CloseAll()
    {
        // cierra todo y reanuda
        pauseUI.SetActive(false);
        controlsUI.SetActive(false);
        abilitiesUI.SetActive(false);
        levelUI.SetActive(true);


        if (levelUI) levelUI.SetActive(true);

        Time.timeScale = 1;
        currentState = MenuState.None;
    }

    // Este método lo enlazas con tu botón “Return to Game”
    public void ReturnToGame() => CloseAll();

    // Guardar y salir, o salir al menú principal
    public void SaveAndExit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public void ExitToMainMenu() => SaveAndExit();
}