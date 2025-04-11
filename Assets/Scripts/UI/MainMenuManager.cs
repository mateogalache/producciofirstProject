//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

   //Carreguem escena segons nom
   public void LoadScene (string sceneName) {
       SceneManager.LoadScene(sceneName);
   }

    public void StartNewGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame(); // Llama a la función para comenzar un nuevo juego
        }
        else
        {
            Debug.LogError("GameManager no está disponible.");
            SceneManager.LoadScene("Level1");
        }
       
    }


    //Tancar joc
    public void ExitGame() {
       Debug.Log("Exiting the game...");
       #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // para el modo play en l'editor'
        #else
            Application.Quit(); // Tanca aplicació fora de l'editor'
        #endif
   }

    public void ContinueGame()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.HasSavedProgress())
            {
                GameManager.Instance.RestartLevel(); // Si hay progreso guardado, continúa desde el checkpoint
            }
            else
            {
                Debug.Log("No hay progreso guardado. Comienza un nuevo juego.");
                StartNewGame(); // Si no hay progreso, se inicia un nuevo juego
            }
        }
        else
        {
            Debug.LogError("GameManager no está disponible.");
        }
    }

}
