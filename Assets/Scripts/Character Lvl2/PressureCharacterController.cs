using UnityEngine;
using TMPro;



public class PressureEntityController : MonoBehaviour
{
    [Header("UI")]
    public GameObject uiMessagePanel;
    public TextMeshProUGUI messageText;

    [Header("Mensajes por defecto")]
    public string[] pressureMessages;
    public float displayDuration = 4f;

    public ParticleSystem particles;
    private int currentMessage = 0;

    private void Awake()
    {
        
        if (uiMessagePanel == null)
        {
            uiMessagePanel = GameObject.FindWithTag("PressurePanel");
            uiMessagePanel.SetActive(false);
            if (uiMessagePanel == null)
                Debug.LogError("[PressureEntity] No encontré ningún objeto con tag 'PressurePanel'");
        }

      
        if (messageText == null && uiMessagePanel != null)
        {
            messageText = uiMessagePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (messageText == null)
                Debug.LogError("[PressureEntity] No encontré TextMeshProUGUI dentro del panel");
        }

    
        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>();

        if (particles == null)
            Debug.LogError("[PressureEntity] Asigna el ParticleSystem en el Inspector o como hijo");

        var main = particles.main;
        main.loop = true;
        particles.Stop();  

        
        if (uiMessagePanel != null)
            uiMessagePanel.SetActive(false);

    }

    public void Appear(Vector3 position, string[] overrideMessages = null,
        float overrideDuration = -1f)
    {

        
        if (overrideMessages != null && overrideMessages.Length > 0)
        {
            pressureMessages = overrideMessages;
            currentMessage = 0;
        }

        transform.position = position;
        //particles.Clear(true);


        if (particles != null)
        {
            particles.transform.position = position;
            particles.Play();
        }

        float delayBeforeText = 1.5f;
        CancelInvoke(nameof(ShowMessage));
        Invoke(nameof(ShowMessage), delayBeforeText);
    }

    private void ShowMessage()
    {
        if (uiMessagePanel != null && pressureMessages.Length > 0)
        {
            string msg = pressureMessages[currentMessage % pressureMessages.Length];
            uiMessagePanel.SetActive(true);
            messageText.text = msg;
            currentMessage++;

            float duration = displayDuration;
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), duration);
        }
    }

    private void HideMessage()
    {
        uiMessagePanel.SetActive(false);
    }
}