using UnityEngine;
using UnityEngine.SceneManagement;

public class MenusManager: MonoBehaviour
{
    // Nom escenes
    [SerializeField] private string menuPausaScene = "PauseMenu";
    [SerializeField] private string menuControlsScene = "ControlsMenu";
    [SerializeField] private string menuHabilitatsScene = "HabilitiesMenu";
    [SerializeField] private string menuPrincipalScene = "MainMenu";
    [SerializeField] private string nivell1Scene = "Level1";

    //private bool isPaused = false;

    private string currentMenu = null; // Menu actual ober
    private bool isMenuOpen = false;  // Si hi ha menu obert

    private static MenusManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
           
    }

    void Update()
    {
        /*
        // Obrir men� de pausa amb la tecla Esc
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentMenu == menuPausaScene)
            {
                ReturnToGame();
            }
            else if (!isMenuOpen)
            {
                OpenMenu(menuPausaScene);
                
            }
          
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentMenu == menuControlsScene)
            {
                ReturnToGame();
            }
            else if (!isMenuOpen)
            {
                OpenMenu(menuControlsScene);
            }
        }

        // DownArrow: Abrir menú de habilidades / volver al juego
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentMenu == menuHabilitatsScene)
            {
                ReturnToGame();
            }
            else if (!isMenuOpen)
            {
                OpenMenu(menuHabilitatsScene);
            }
        }
    }

    // Carregar escena del men� seleccionat
    private void OpenMenu(string nomMenu)
    {
        if (SceneManager.GetSceneByName(nomMenu).isLoaded)
            return;

        SceneManager.LoadScene(nomMenu, LoadSceneMode.Additive); // Carregar el men� sobre el nivell actual
        currentMenu = nomMenu;
        isMenuOpen = true;
        Time.timeScale = 0; // Pausar el joc
        RemoveExtraAudioListeners();
        */

        if (Input.GetKeyDown(KeyCode.A)) ToggleMenu(menuPausaScene);
        if (Input.GetKeyDown(KeyCode.W)) ToggleMenu(menuControlsScene);
        if (Input.GetKeyDown(KeyCode.D)) ToggleMenu(menuHabilitatsScene);

    }

    private void ToggleMenu(string menuName)
    {
        if (currentMenu == menuName)
            ReturnToGame();
        else if (!isMenuOpen)
            //OpenMenu(menuName);
            StartCoroutine(OpenMenu(menuName));
    }

    private System.Collections.IEnumerator OpenMenu(string menuName)
    {
        //if (SceneManager.GetSceneByName(menuName).isLoaded)
        //    return;

        //SceneManager.LoadScene(menuName, LoadSceneMode.Additive);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(menuName, LoadSceneMode.Additive);

        // Esperar hasta que se cargue completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(menuName);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        currentMenu = menuName;
        isMenuOpen = true;
        Time.timeScale = 0;
        RemoveExtraAudioListeners();
        RemoveExtraEventSystems();

       
    }

    private void RemoveExtraEventSystems()
    {
        UnityEngine.EventSystems.EventSystem[] systems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();

        if (systems.Length > 1)
        {
            // Dejamos activo solo el primero
            for (int i = 1; i < systems.Length; i++)
            {
                systems[i].gameObject.SetActive(false);
            }
        }
    }

    // Tornar al joc
    public void CloseMenu()
    {
        if (currentMenu != null) {
             SceneManager.UnloadSceneAsync(currentMenu); // Tanca el men� actual
            currentMenu = null;
        }
        isMenuOpen = false;
        Time.timeScale = 1; //reanudar temps de joc
       
    }

    public void SaveAndExit()
    {

        Debug.Log("SaveAndExit presionado");

        // Tornar al menu principal
        Time.timeScale = 1; // Reanudar temps
        SceneManager.LoadScene(menuPrincipalScene); // Carregar men� principal
    }

    // Sortir al men� principal
    public void ExitToMainMenu()
    {
        Debug.Log("ExitToMainMenu presionado");
        Time.timeScale = 1; // Reanudar temps
        SceneManager.LoadScene(menuPrincipalScene); // Carregar men� principal
    }

    // Tornar al nivell 1
    public void ReturnToGame()
    {
        //Time.timeScale = 1; // Reanudar temps
        //SceneManager.LoadScene(nivell1Scene); // Carregar el nivell 1 (Sample Scene)

        Debug.Log("ReturnToGame presionado");
        Time.timeScale = 1;
        CloseMenu();
        //SceneManager.LoadScene(nivell1Scene);
    }

    private void RemoveExtraAudioListeners()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();

        if (listeners.Length > 1)
        {
            //Debug.LogWarning("Se detectaron múltiples AudioListeners. Desactivando los adicionales.");

            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false; // En lugar de destruirlos, los desactivamos
            }
        } 
    }


}