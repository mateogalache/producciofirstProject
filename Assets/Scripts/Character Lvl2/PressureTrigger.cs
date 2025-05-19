using UnityEngine;

public class PressureTrigger : MonoBehaviour
{

    [Header("Prefab de la entidad de partículas")]
    public PressureEntityController pressureEntity;

    public Vector3 spawnOffset = new Vector3(0, 1, 0);

    [Header("Mensaje y duración para este trigger")]
    public string[] customMessages;
    public float customDisplayDuration = 10f;

    private bool triggered = false;

    /*
    private void Awake()
    {
        // Aseguramos que el collider sea trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            pressureEntity.Appear(transform.position + spawnOffset, customMessages, customDisplayDuration);
            triggered = true;
        }
    }*/

    private void Awake()
    {
        // Asegúrate de que tu BoxCollider2D tenga Is Trigger = true
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // 1) Calcula dónde spawnear
        Vector3 spawnPos = transform.position + spawnOffset;

        // 2) Instancia el prefab en esa posición
        var instance = Instantiate(
            pressureEntity,
            spawnPos,
            Quaternion.identity
        );

        // 3) Le dices que aparezca con tus mensajes y duración
        instance.Appear(spawnPos, customMessages, customDisplayDuration);

        triggered = true;
    }
}