using UnityEngine;

public class PressureTrigger : MonoBehaviour
{
    public PressureEntityController pressureEntity;
    public Vector3 spawnOffset;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Trigger] Activado por el jugador");
            pressureEntity.Appear(transform.position + spawnOffset);
            triggered = true;
        }
    }
}