using UnityEngine;
using TMPro;



public class PressureEntityController : MonoBehaviour
{
    [Header("UI")]
    public GameObject uiMessagePanel; 
    public TextMeshProUGUI messageText;

    [Header("Mensajes por defecto")]
    public string[] pressureMessages;
    public float displayDuration = 10f;

    public ParticleSystem particles;
    private int currentMessage = 0;

    private void Awake()
    {
        /*
        particles = GetComponentInChildren<ParticleSystem>();
        if (particles == null)
            Debug.LogError("[PressureEntity] ¡No encontré el ParticleSystem en los hijos!");

        var main = particles.main;
        main.loop = true;

        particles.Stop();

        if (uiMessagePanel != null)
        {
            uiMessagePanel.SetActive(false);
        }*/
        // 1) Buscar el panel si no está asignado
        if (uiMessagePanel == null)
        {
            uiMessagePanel = GameObject.FindWithTag("PressurePanel");
            uiMessagePanel.SetActive(false);
            if (uiMessagePanel == null)
                Debug.LogError("[PressureEntity] No encontré ningún objeto con tag 'PressurePanel'");
        }

        // 2) Buscar el TMP Text dentro del panel si no está asignado
        if (messageText == null && uiMessagePanel != null)
        {
            messageText = uiMessagePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (messageText == null)
                Debug.LogError("[PressureEntity] No encontré TextMeshProUGUI dentro del panel");
        }

        // 3) Configurar ParticleSystem
        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>();

        if (particles == null)
            Debug.LogError("[PressureEntity] Asigna el ParticleSystem en el Inspector o como hijo");

        var main = particles.main;
        main.loop = true;
        particles.Stop();  // No emite hasta que llamemos a Play()

        // 4) Ocultar UI al inicio
        if (uiMessagePanel != null)
            uiMessagePanel.SetActive(false);

    }

    public void Appear(Vector3 position, string[] overrideMessages = null,
        float overrideDuration = -1f)
    {

        // Si me pasan mensajes nuevos, los uso
        if (overrideMessages != null && overrideMessages.Length > 0)
        {
            pressureMessages = overrideMessages;
            currentMessage = 0;
        }

        transform.position = position;
        //particles.Clear(true);
        particles.transform.position = position;
        particles.Play();

        if (uiMessagePanel != null && pressureMessages.Length > 0)
        {
            string msg = pressureMessages[currentMessage % pressureMessages.Length];
            uiMessagePanel.SetActive(true);
            messageText.text = msg;
            currentMessage++;

            // Duración a usar
            float duration = (overrideDuration > 0f) ? overrideDuration : displayDuration;
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), duration);

            //CancelInvoke(nameof(HideMessage));
            //Invoke(nameof(HideMessage), displayDuration);
        }
    }

    private void HideMessage()
    {
        uiMessagePanel.SetActive(false);
    }
}