using UnityEngine;

public class CheckpointLvl2 : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            if (GameManagerLvl2.Instance != null)
            {
                GameManagerLvl2.Instance.UpdateCheckpoint(transform.position);
                isActivated = true;
                ActivateCheckpointVisual();
            }
            else
            {
                Debug.LogError("GameManagerLvl2 no está presente en la escena.");
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
