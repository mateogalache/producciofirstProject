using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    public TutorialUI tutorialUI;

    private void Start()
    {
        if (tutorialUI == null)
        {
            tutorialUI = FindObjectOfType<TutorialUI>(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
           
            if (GameManager.Instance != null)
            {
                Vector3 checkpointPosition = transform.position;
                GameManager.Instance.UpdateCheckpoint(checkpointPosition);
                isActivated = true;

               
                ActivateCheckpointVisual();

                if (tutorialUI != null)
                {
                    tutorialUI.ShowMessage("�Partida guardada!");
                } else
                {
                    Debug.LogError("TutorialUI no est� asignado en el Inspector.");
                }

                   
            }
            else
            {
                Debug.LogError("GameManager no est� presente en la escena.");
            }
        }
    }

    private void ActivateCheckpointVisual()
    {
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.green; 
        }

        
    }

}
