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

   //Tancar joc
   public void ExitGame() {
       Debug.Log("Exiting the game...");
       #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // para el modo play en l'editor'
        #else
            Application.Quit(); // Tanca aplicació fora de l'editor'
        #endif
   }


/*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
