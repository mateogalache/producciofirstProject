using UnityEngine;
using UnityEngine.SceneManagement;

public class MenusManager: MonoBehaviour
{
    // Nom escenes
    [SerializeField] private string menuPausaScene = "PauseMenu";
    [SerializeField] private string menuControlsScene = "ControlsMenu";
    [SerializeField] private string menuHabilitatsScene = "HabilitiesMenu";
    [SerializeField] private string menuPrincipalScene = "MenuPrincipal";
    [SerializeField] private string nivell1Scene = "Sample Scene";

    //private bool isPaused = false;

    private string currentMenu = null; // Menu actual obert
    private bool isMenuOpen = false;  // Si hi ha menu obert

    void Update()
    {
        // Obrir menú de pausa amb la tecla Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMenuOpen)
            {
                OpenMenu(menuPausaScene);
                //isPaused = true; 
            } else {
                CloseMenu();
            }
          
        }

        // Obrir menú de controls amb fletxa Up
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isMenuOpen)
        {
            OpenMenu(menuControlsScene);
        }

        // Obrir menú d`habilitats amb fletxa Down
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isMenuOpen)
        {
            OpenMenu(menuHabilitatsScene);
        }
    }

    // Carregar escena del menú seleccionat
    private void OpenMenu(string nomMenu)
    {
        SceneManager.LoadScene(nomMenu, LoadSceneMode.Additive); // Carregar el menú sobre el nivell actual
        currentMenu = nomMenu;
        isMenuOpen = true;
        Time.timeScale = 0; // Pausar el joc
    }

    // Tornar al joc
    public void CloseMenu()
    {
        if (currentMenu != null) {
             SceneManager.UnloadSceneAsync(currentMenu); // Tanca el menú actual
            currentMenu = null;
        }
        isMenuOpen = false;
        Time.timeScale = 1; //reanudar temps de joc
       
    }

    // Guardar i sortir al menú principal
    public void SaveAndExit()
    {
        // FALTA afegir funcionalitat per guardar el progrés
        Debug.Log("Partida guardada!");

        // Tornar al menú principal
        Time.timeScale = 1; // Reanudar temps
        SceneManager.LoadScene(menuPrincipalScene); // Carregar menú principal
    }

    // Sortir al menú principal
    public void ExitToMainMenu()
    {
        Time.timeScale = 1; // Reanudar temps
        SceneManager.LoadScene(menuPrincipalScene); // Carregar menú principal
    }

    // Tornar al nivell 1
    public void ReturnToGame()
    {
        Time.timeScale = 1; // Reanudar temps
        SceneManager.LoadScene(nivell1Scene); // Carregar el nivell 1 (Sample Scene)
    }

}