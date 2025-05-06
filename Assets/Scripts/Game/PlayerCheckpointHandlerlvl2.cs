using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCheckpointHandler : MonoBehaviour
{
    [Header("Layer de peligro (Hazard)")]
    public LayerMask hazardLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Respawnea si colisiona con objeto de layer \"Hazard\"
        if (((1 << collision.gameObject.layer) & hazardLayer) != 0)
        {
            GameManagerLvl2.Instance.RespawnPlayer(gameObject);
        }
    }
}
