using UnityEngine;
using TMPro;



public class PressureEntityController : MonoBehaviour
{
    public GameObject uiMessagePanel; // Un panel UI con mensaje
    public TextMeshProUGUI messageText;
    public string[] pressureMessages;
    public float displayDuration = 3f;

    private ParticleSystem particles;
    private int currentMessage = 0;

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        if (uiMessagePanel != null)
            uiMessagePanel.SetActive(false);
    }

    public void Appear(Vector3 position)
    {
        transform.position = position;
        Debug.Log("[PressureEntity] Apareciendo en " + position);
        particles.Play();

        if (uiMessagePanel != null && pressureMessages.Length > 0)
        {
            Debug.Log("[PressureEntity] Mostrando mensaje: " + pressureMessages[currentMessage % pressureMessages.Length]);
            uiMessagePanel.SetActive(true);
            messageText.text = pressureMessages[currentMessage % pressureMessages.Length];
            currentMessage++;

            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), displayDuration);
        }
    }

    private void HideMessage()
    {
        uiMessagePanel.SetActive(false);
    }
}