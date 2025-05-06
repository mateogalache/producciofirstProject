using UnityEngine;

public class CheckpointManagerlvl2 : MonoBehaviour
{
    public static CheckpointManagerlvl2 Instance { get; private set; }
    public Transform[] spawnPoints;

    private Transform currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Por defecto, usamos el primer punto
        if (spawnPoints.Length > 0)
        {
            currentCheckpoint = spawnPoints[0];
        }
    }

    public void SetCheckpoint(Transform checkpointTransform)
    {
        currentCheckpoint = checkpointTransform;
    }

    public void RespawnPlayer(GameObject player)
    {
        if (currentCheckpoint != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
            player.transform.position = currentCheckpoint.position;
        }
    }
}
