using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip jumpSound;
    
    public AudioClip collectStarSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
        else
        {
            Debug.LogWarning("Jump Sound no está asignado en PlayerAudio.");
        }
    }

    

    public void PlayCollectStarSound()
    {
        if (collectStarSound != null)
        {
            audioSource.PlayOneShot(collectStarSound);
        }
        else
        {
            Debug.LogWarning("Collect Star Sound no está asignado en PlayerAudio.");
        }
    }
}
