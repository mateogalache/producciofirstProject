using UnityEngine;

public class PressureTrigger : MonoBehaviour
{

    [Header("Prefab de la entidad de partículas")]
    public PressureEntityController pressureEntity;

    public Vector3 spawnOffset = new Vector3(0, 1, 0);

    [Header("Mensaje y duración para este trigger")]
    public string[] customMessages;
    public float customDisplayDuration = 2f;

    private bool triggered = false;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        Vector3 spawnPos = transform.position + spawnOffset;

        var instance = Instantiate(
            pressureEntity,
            spawnPos,
            Quaternion.identity
        );

        instance.Appear(spawnPos, customMessages, customDisplayDuration);

        triggered = true;
    }
}