using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    public string starID;
    private bool collected = false;

    private void Awake()
    {
        if (string.IsNullOrEmpty(starID))
        {
            starID = gameObject.name;
        }
    }

    private void Start()
    {
        if (StarTracker.Instance != null && StarTracker.Instance.HasCollected(starID))
        {
            Debug.Log($"[StarCollectible]  Ocultando estrella ya recogida: {starID}");
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[StarCollectible] Trigger con {other.name}");

        if (collected || !other.CompareTag("Player")) return;

        if (StarTracker.Instance.AddStar(starID))
        {
            Debug.Log($"[StarCollectible]  Estrella válida recogida: {starID}");
            collected = true;
            StarUIManager.Instance?.AddStar(transform.position);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"[StarCollectible]  Estrella duplicada ignorada: {starID}");
        }
    }
}
